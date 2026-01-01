using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.RoleViewModels;

public class ManageUserRolesViewModel
{
  public required string UserId { get; set; }
  public required string UserName { get; set; }
  public required List<RoleViewModel> Roles { get; set; }
}

public class RoleViewModel
{
  public required string RoleId { get; set; }
  public required string RoleName { get; set; }
  public bool IsSelected { get; set; }
}