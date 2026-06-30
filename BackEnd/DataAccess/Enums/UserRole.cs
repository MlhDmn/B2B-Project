namespace B2B_Proje.DataAccess.Enums
{
    [Flags]
    public enum UserRole
    {
        ViewProducts = 1,
        AddProducts = 2,
        DeleteProducts = 4,
        EditProducts = 8,
        ManageCategories = 16,
        ManageUsers = 32,
        Admin = ViewProducts | AddProducts | DeleteProducts | EditProducts | ManageCategories | ManageUsers

    }
}