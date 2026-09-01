namespace ApiAutomationFramework.Constants;

public static class ApiEndpoints
{
    public static class Users
    {
        public const string GetAll = "/users";
        public const string GetById = "/users/{id}";
        public const string Create = "/users";
        public const string Update = "/users/{id}";
        public const string Delete = "/users/{id}";
    }

    public static class Posts
    {
        public const string GetAll = "posts";
        public const string GetById = "posts/{id}";
        public const string Create = "posts";
        public const string Update = "posts/{id}";
        public const string Delete = "posts/{id}";
    }

    public static class Authentication
    {
        public const string Login = "/login";
        public const string Register = "/register";
    }
}