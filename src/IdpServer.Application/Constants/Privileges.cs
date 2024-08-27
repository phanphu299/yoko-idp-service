namespace IdpServer.Application.Constant
{
    public static class Privileges
    {
        public static class Users
        {
            public const string ENTITY_NAME = "user";
            public static class Rights
            {
                public const string VIEW_USER = "read_user";
                public const string CREATE_USER = "write_user";
                public const string EDIT_USER = "write_user";
                public const string DELETE_USER = "delete_user";
            }
        }

        public static class Project
        {
            public const string ENTITY_NAME = "project";
        }
    }
}
