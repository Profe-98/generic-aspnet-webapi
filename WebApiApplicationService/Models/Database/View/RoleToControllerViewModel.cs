using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class RoleToControllerViewModel : AbstractModel
    {
        #region Private
        private string _route = null;
        private string[] _routeSegments = null;
        private int[] _routeSegmentsIndexOfValues = null;
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnPropertyAttribute("role_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role")]
        [DatabaseColumnPropertyAttribute("role", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Role { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_method")]
        [DatabaseColumnPropertyAttribute("http_method", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string HttpMethod { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("crud")]
        [DatabaseColumnPropertyAttribute("crud", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Crud { get; set; }

        [JsonPropertyName("flag")]
        [DatabaseColumnPropertyAttribute("flag", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int Flag { get; set; } = GeneralDefs.NotFoundResponseValue;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnPropertyAttribute("route", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Route
        {
            get
            {
                return _route;
            }
            set
            {
                if(value != null)
                {

                    _route = value.ToLower();

                    _routeSegments = _route.Split(new string[] { "/" }, StringSplitOptions.None);
                    List<int> matchIndexes = new List<int>();
                    for (int i = 0; i < _routeSegments.Length; i++)
                    {
                        string valueTmp = _routeSegments[i];
                        Match match = Regex.Match(valueTmp, BackendAPIDefinitionsProperties.UriValueWildCardExtractRegEx);
                        if (match.Success)
                        {
                            matchIndexes.Add(i);
                        }
                    }
                    _routeSegmentsIndexOfValues = matchIndexes.ToArray();
                }

            }
        }


        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute("controller_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller")]
        [DatabaseColumnPropertyAttribute("controller", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Controller { get; set; }

        [JsonPropertyName("max_request_per_hour")]
        [DatabaseColumnPropertyAttribute("max_request_per_hour", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int MaxRequestPerHour { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonPropertyName("max_time_after_request_in_ms")]
        [DatabaseColumnPropertyAttribute("max_time_after_request_in_ms", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int MaxTimeAfterRequestInMs { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonIgnore]
        public string[] RouteSegments
        {
            get
            {
                return _routeSegments;
            }
        }

        [JsonIgnore]
        public int[] RouteSegmentsValueIndexer
        {
            get
            {
                return _routeSegmentsIndexOfValues;
            }
        }



        #region Ctor & Dtor
        public RoleToControllerViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
