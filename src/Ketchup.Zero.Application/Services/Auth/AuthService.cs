using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Permission;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Profession.Utilis;

namespace Ketchup.Zero.Application.Services.Auth
{
    public class AuthService : Permission.Auth.AuthBase
    {
        private readonly IEfCoreRepository<Domain.SysUser, int> _sysUser;

        public AuthService(IEfCoreRepository<Domain.SysUser, int> sysUser)
        {
            _sysUser = sysUser;
        }

        public override Task<TokenResponse> Login(TokenRequst request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "用户名不能为空"));

            if (string.IsNullOrEmpty(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "密码不能为空"));

            request.Password = request.Password.Get32MD5One();

            var sysUser =
                _sysUser.SingleOrDefault(item => item.UserName == request.Name && item.Password == request.Password);

            if (sysUser == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "用户名密码错误"));

            return Task.FromResult(new TokenResponse()
            {
                UserName = sysUser.UserName,
                RoleId = sysUser.RoleId,
                UserId = sysUser.Id
            });

        }

        //private string GenerateToken(Config.AppConfig appConfig, HttpContext context)
        //{

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.Zero.Secret));

        //    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, "simple"),
        //        new Claim("Role", "1")
        //    };

        //    identity.AddClaims(claims);
        //    context.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).Wait();

        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //        issuer: appConfig.Zero.Key,
        //        notBefore: DateTime.Now,
        //        expires: DateTime.Now.AddSeconds(appConfig.Zero.AuthExpired),
        //        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        //    );

        //    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        //    return jwtToken;
        //}
    }
}
