using AutoMapper;
using PRN232.Lab2.CoffeeStore.Services.Helpers;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class UserService : CrudService<AppUser, UserBM>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, JwtHelper jwtHelper)
            : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtHelper = jwtHelper;

        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<AppUser>
            GetRepository() => UnitOfWork.UserRepository;

        public async Task<PagedResult<UserBM>> GetPagedFilteredAsync(UserBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.UserRepository;

            var repositoryFilter = new AppUser
            {
                UserId = filter?.Userid ?? 0,
                Username = filter?.Username,
                Email = filter?.Email,
                Role = filter?.Role
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<UserBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<UserBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public async Task<ApiResponse<AuthResultModel>> RegisterAsync(string username, string email, string password)
        {
            var existingByUsername = await _unitOfWork.UserRepository.GetByUsernameAsync(username);
            if (existingByUsername != null)
                return ApiResponse<AuthResultModel>.FailResponse("Username already exists", 409);
            var existingByEmail = await _unitOfWork.UserRepository.GetByEmailAsync(email);
            if (existingByEmail != null)
                return ApiResponse<AuthResultModel>.FailResponse("Email already exists", 409);

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new AppUser
            {
                Username = username.Trim(),
                Email = email.Trim(),
                Password = passwordHash,
                Salt = passwordSalt,
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.UserRepository.AddAsync(user);
            var savedCreate = await _unitOfWork.SaveChangesAsync();
            if (savedCreate <= 0)
                return ApiResponse<AuthResultModel>.FailResponse("Register failed", 500);

            var result = await GenerateAuthResultAsync(user);
            return ApiResponse<AuthResultModel>.SuccessResponse(result, "Registered", 201);
        }

        public async Task<ApiResponse<AuthResultModel>> LoginAsync(string usernameOrEmail, string password)
        {
            AppUser? user = await _unitOfWork.UserRepository.GetByUsernameAsync(usernameOrEmail);
            user ??= await _unitOfWork.UserRepository.GetByEmailAsync(usernameOrEmail);
            if (user == null || !VerifyPasswordHash(password, user.Password, user.Salt))
                return ApiResponse<AuthResultModel>.FailResponse("Invalid username/email or password", 400);
            if ((bool)!user.IsActive)
                return ApiResponse<AuthResultModel>.FailResponse("User is inactive", 403);

            var result = await GenerateAuthResultAsync(user);
            return ApiResponse<AuthResultModel>.SuccessResponse(result, "Logged in", 200);
        }

        public async Task<ApiResponse<AuthResultModel>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return ApiResponse<AuthResultModel>.FailResponse("Invalid refresh token", 400);

            var result = await GenerateAuthResultAsync(user);
            return ApiResponse<AuthResultModel>.SuccessResponse(result, "Token refreshed", 200);
        }

        public async Task<ApiResponse<object>> LogoutAsync(string refreshToken)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                return ApiResponse<object>.FailResponse("Invalid refresh token", 400);
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
             _unitOfWork.UserRepository.Update(user);
            var savedUpdate = await _unitOfWork.SaveChangesAsync();
            if (savedUpdate <= 0)
                return ApiResponse<object>.FailResponse("Logout failed", 500);
            return ApiResponse<object>.SuccessResponse(null, "Logged out", 204);
        }

        private async Task<AuthResultModel> GenerateAuthResultAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var accessToken = _jwtHelper.GenerateAccessToken(claims);
            var accessJwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var accessExpires = accessJwt.ValidTo.ToUniversalTime();
            var (refreshToken, refreshExpiresAt) = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpiresAt;
             _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new AuthResultModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = accessExpires,
                RefreshTokenExpiresAtUtc = refreshExpiresAt
            };
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(storedHash);
        }

        public IQueryable<UserBM> GetODataQueryable()
        {
            var repo = UnitOfWork.UserRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<UserBM>(query);
        }
    }
}
