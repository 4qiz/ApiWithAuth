namespace AuthWith2Fa.Dtos.Response
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessful { get; set; }

        public IEnumerable<string> Errors { get; set; } = [];
    }
}
