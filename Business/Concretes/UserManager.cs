using AutoMapper;
using Business.Abstracts;
using Business.Dtos.Requests.User;
using Business.Dtos.Responses.User;
using Business.Rules;
using Core.Aspects.Autofac.SecuredOperation;
using Core.DataAccess.Paging;
using Core.Entities;
using DataAccess.Abstracts;
using Entities.Concretes;
using Serilog;

namespace Business.Concretes
{
    public class UserManager : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDal _userDal;
        private readonly UserBusinessRules _userBusinessRules;

        public UserManager(IMapper mapper, IUserDal userDal, UserBusinessRules userBusinessRules)
        {
            _mapper = mapper;
            _userDal = userDal;
            _userBusinessRules = userBusinessRules;

            // Serilog'un yapılandırması
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(@"C:\Logs\user-manager-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public async Task<CreatedUserResponse> AddAsync(CreateUserRequest createUserRequest)
        {
            try
            {
                User user = _mapper.Map<User>(createUserRequest);
                var createdUser = await _userDal.AddAsync(user);
                CreatedUserResponse result = _mapper.Map<CreatedUserResponse>(createdUser);

                // Loglama
                Log.Information("User added: {@CreateUserRequest}", createUserRequest);

                return result;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while adding user");
                throw; // Hatanın yeniden fırlatılması
            }
        }

        [SecuredOperation("admin")]
        public async Task<DeletedUserResponse> DeleteByIdAsync(Guid id)
        {
            try
            {
                User user = await _userBusinessRules.CheckIfExistsById(id);
                var deletedUser = await _userDal.DeleteAsync(user);
                DeletedUserResponse deletedUserResponse = _mapper.Map<DeletedUserResponse>(deletedUser);

                // Loglama
                Log.Information("User deleted by Id: {UserId}", id);

                return deletedUserResponse;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while deleting user by Id: {UserId}", id);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        [SecuredOperation("admin")]
        public async Task<DeletedUserResponse> DeleteByMailAsync(string email)
        {
            try
            {
                User user = await _userBusinessRules.CheckIfExistsByMail(email);
                var deletedUser = await _userDal.DeleteAsync(user);
                DeletedUserResponse deletedUserResponse = _mapper.Map<DeletedUserResponse>(deletedUser);

                // Loglama
                Log.Information("User deleted by email: {Email}", email);

                return deletedUserResponse;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while deleting user by email: {Email}", email);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        public async Task<GetUserResponse> GetByIdAsync(Guid id)
        {
            try
            {
                User user = await _userDal.GetAsync(u => u.Id == id);

                // Loglama
                Log.Information("User retrieved by Id: {UserId}", id);

                return _mapper.Map<GetUserResponse>(user);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while retrieving user by Id: {UserId}", id);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        public async Task<User> GetByMailAsync(string mail, bool withDeleted)
        {
            try
            {
                var result = await _userDal.GetAsync(u => u.Email == mail, withDeleted: withDeleted);

                // Loglama
                Log.Information("User retrieved by email: {Email}", mail);

                return result;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while retrieving user by email: {Email}", mail);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        [SecuredOperation("admin")]
        public async Task<IPaginate<GetListUserResponse>> GetListAsync(PageRequest pageRequest)
        {
            try
            {
                var result = await _userDal.GetListAsync(index: pageRequest.PageIndex, size: pageRequest.PageSize);

                // Loglama
                Log.Information("User list retrieved");

                return _mapper.Map<Paginate<GetListUserResponse>>(result);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while retrieving user list");
                throw; // Hatanın yeniden fırlatılması
            }
        }

    public async Task<UpdatedUserResponse> UpdateAsync(UpdateUserRequest updateUserRequest)
        {
            try
            {
                User user = await _userBusinessRules.CheckIfExistsById(updateUserRequest.Id);
                _mapper.Map(updateUserRequest, user);
                var updatedUser = await _userDal.UpdateAsync(user);
                UpdatedUserResponse updatedUserResponse = _mapper.Map<UpdatedUserResponse>(updatedUser);

                // Loglama
                Log.Information("User updated: {@UpdateUserRequest}", updateUserRequest);

                return updatedUserResponse;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while updating user: {@UpdateUserRequest}", updateUserRequest);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        public async Task<bool> ActivateUserAsync(string email)
        {
            try
            {
                User user = await _userDal.GetAsync(u => u.Email == email, withDeleted: true);
                user.DeletedDate = null;
                await _userDal.UpdateAsync(user);

                // Loglama
                Log.Information("User activated by email: {Email}", email);

                return true;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while activating user by email: {Email}", email);
                throw; // Hatanın yeniden fırlatılması
            }
        }

        public List<IOperationClaim> GetClaims(IUser user)
        {
            try
            {
                return _userDal.GetClaims(user);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while getting user claims");
                throw; // Hatanın yeniden fırlatılması
            }
        }

        public async Task<GetByMailUserResponse> GetByMailUserAsync(string mail)
        {
            try
            {
                User result = await _userDal.GetAsync(u => u.Email == mail);

                // Loglama
                Log.Information("User retrieved by mail: {Email}", mail);

                return _mapper.Map<GetByMailUserResponse>(result);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Log.Error(ex, "Error occurred while retrieving user by mail: {Email}", mail);
                throw; // Hatanın yeniden fırlatılması
            }
        }
    }
}
