using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace InQuant.Navigation
{
    public interface INavigationManager
    {
        IEnumerable<MenuItem> BuildMenu(string name, ActionContext context);
    }
}
