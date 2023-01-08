using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ApiModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        
        //db füllen
        //relation klassen intern sollen nicht json serializable sein, tabellen und column attribute mit db abgleichen und auf gleichen stand bringen + auth token caching mit lifetime + alles von CustomAuthorizationMiddleware in cache 


        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("controller_route_pattern", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string RouterPattern { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("controller_area_pattern", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string AreaPattern { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Name { get; set; }


        [JsonIgnore]
        public List<ControllerModel> AvaibleControllers = new List<ControllerModel>();

        [JsonIgnore]
        public ControllerModel AuthController
        {
            get
            {
                List<ControllerModel> controllers = this.AvaibleControllers.FindAll(x => x.IsAuthcontroller);
                if (controllers.Count > 1)
                {
                    throw new NotSupportedException("you cant host more than 1 auth controllers for an api-area");
                }

                return controllers?.Count != 0 ?
                    controllers[0] : null;
            }
        }
        [JsonIgnore]
        public ControllerModel ErrController
        {
            get
            {
                List<ControllerModel> controllers = this.AvaibleControllers.FindAll(x => x.IsErrorController);
                if (controllers.Count > 1)
                {
                    throw new NotSupportedException("you cant host more than 1 error controllers for an api-area");
                }

                return controllers?.Count != 0 ?
                    controllers[0] : null;
            }
        }
        #region Ctor & Dtor
        public ApiModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        public Uri GetRouteToController(ControllerModel controller)
        {
            if (controller == null)
                return null;

            return new Uri(("/"+this.Name + "/" + controller.Name + "/").ToLower(),UriKind.Relative);
        }
        #endregion Methods
    }
}
