using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace WebApp.Filters;

public class CheckBoxReq : ValidationAttribute
{
	public override bool IsValid(object? value) => value is bool b && b;

}
