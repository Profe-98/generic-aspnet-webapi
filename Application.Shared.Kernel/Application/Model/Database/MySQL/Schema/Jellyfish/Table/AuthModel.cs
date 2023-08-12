using System.Text.Json.Serialization;
using System.Net;
using MySql.Data.MySqlClient;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table
{
    [Serializable]
    public class AuthModel : Application.Model.Database.MySQL.Schema.ApiGateway.Table.AuthModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;


        [JsonPropertyName("test")]
        [SensitiveData("user,admin,root")]
        public string Test { get; set; } = "testeintrag";

        #region Ctor & Dtor
        public AuthModel()
        {

        }
        public AuthModel(Application.Model.Database.MySQL.Schema.ApiGateway.Table.AuthModel authModel)
        {
            if (authModel == null)
                return;
            Uuid = authModel.Uuid;
            UserUuid = authModel.UserUuid;
            Ipv4 = authModel.Ipv4;
            Ipv6 = authModel.Ipv6;
            RemotePort = authModel.RemotePort;
            LocalPort = authModel.LocalPort;
            Ipv4Local = authModel.Ipv4Local;
            Ipv6Local = authModel.Ipv6Local;
            Token = authModel.Token;
            TokenExpires = authModel.TokenExpires;
            RefreshToken = authModel.RefreshToken;
            RefreshTokenExpires = authModel.RefreshTokenExpires;
            UserAgent = authModel.UserAgent;
            LogoutTime = authModel.LogoutTime;
            IsAdmin = authModel.IsAdmin;
            UserModel = new UserModel(authModel.UserModel);
        }

        #endregion Ctor & Dtor
        #region Methods
        private IPAddress ConvertStringToIp(string ipStr)
        {
            if (IPAddress.TryParse(Ipv4Local, out IPAddress address))
            {
                return address;
            }
            return null;
        }
        #endregion Methods
    }
}
