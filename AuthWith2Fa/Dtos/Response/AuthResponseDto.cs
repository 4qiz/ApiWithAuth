namespace AuthWith2Fa.Dtos.Response
{
    public class AuthResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
