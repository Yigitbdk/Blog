namespace BlogApp.Dto
{
    public class UserWithRolesDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
