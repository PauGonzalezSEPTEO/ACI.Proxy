using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Proxy.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountsController : JwtControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpDelete]
        [Route("delete-user-api-key-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserApiKeyDto>> DeleteUserApiKeyByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            UserApiKeyDto userApiKeyDto = await _accountService.DeleteUserApiKeysByIdAsync(id, cancellationToken);
            return Ok(userApiKeyDto);
        }

        [HttpPost]
        [Route("generate-user-api-key")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GenerateUserApiKeyResultDto>> GenerateUserApiKeyAsync(CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            GenerateUserApiKeyResultDto generateApiKeyResultDto = await _accountService.GenerateUserApiKeyAsync(name, cancellationToken);
            if (!generateApiKeyResultDto.HasErrors)
            {
                return Ok(generateApiKeyResultDto.ApiKey);
            }
            else
            {
                return BadRequest(generateApiKeyResultDto.Errors);
            }
        }

        [HttpGet("get-last-12-hours-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast12HoursUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast12HoursUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-14-days-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast14DaysUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast14DaysUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-3-hours-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast3HoursUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast3HoursUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-6-hours-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast6HoursUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast6HoursUserApiUsageStatisticsAsync();
             return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-7-days-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast7DaysUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast7DaysUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-day-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLastDayUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLastDayUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-hour-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLastHourUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLastHourUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-month-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLastMonthUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLastMonthUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-3-months-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast3MonthsUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast3MonthsUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-6-months-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLast6MonthsUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLast6MonthsUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet("get-last-year-user-api-usage-statistics")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLastYearUserApiUsageStatisticsAsync()
        {
            var userApiUsageStatistics = await _accountService.GetLastYearUserApiUsageStatisticsAsync();
            return Ok(userApiUsageStatistics);
        }

        [HttpGet]
        [Route("me")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> MeAsync(CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            AccountResultDto accountResultDto = await _accountService.GetAccountAsync(name, cancellationToken);
            if (!accountResultDto.HasErrors)
            {
                return Ok(accountResultDto.Account);
            }
            else
            {
                return BadRequest(accountResultDto.Errors);
            }
        }

        [HttpPost]
        [Route("read-user-api-keys-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<UserApiKeyDto>>> ReadUserApiKeysDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<UserApiKeyDto> userApiKeysDto = await _accountService.ReadUserApiKeysDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(userApiKeysDto);
        }

        [HttpGet]
        [Route("read-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> ReadByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto accountResultDto = await _accountService.GetAccountAsync(id, cancellationToken);
            if (!accountResultDto.HasErrors)
            {
                return Ok(accountResultDto.Account);
            }
            else
            {
                return BadRequest(accountResultDto.Errors);
            }
        }

        [HttpDelete]
        [Route("revoke-user-api-key-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserApiKeyDto>> RevokeUserApiKeyByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            UserApiKeyDto userApiKeyDto = await _accountService.RevokeUserApiKeysByIdAsync(id, cancellationToken);
            return Ok(userApiKeyDto);
        }

        [HttpPut]
        [Route("update-profile-details")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProfileDetailsDto>> UpdateProfileDetailsAsync([FromBody] ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            UpdateProfileDetailsResultDto updateProfileDetailsResultDto = await _accountService.UpdateProfileDetailsAsync(name, profileDetailsDto, cancellationToken);
            if (!updateProfileDetailsResultDto.HasErrors)
            {
                return Ok(profileDetailsDto);
            }
            return BadRequest(updateProfileDetailsResultDto.Errors);            
        }
    }
}
