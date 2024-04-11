namespace AdminGateway.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
