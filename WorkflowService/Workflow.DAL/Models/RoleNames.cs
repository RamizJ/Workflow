﻿using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public static class RoleNames
    {
        public const string ADMINISTRATOR_ROLE = "ADMINISTRATOR_ROLE";
        public const string READ_ALL_SCOPES_ROLE = "READ_ALL_SCOPES_ROLE";
        public const string CREATE_SCOPE_ROLE = "CREATE_SCOPE_ROLE";
        public const string CREATE_GOAL_ROLE = "CREATE_GOAL_ROLE";
        public const string REMOVE_GOAL_ROLE = "REMOVE_GOAL_ROLE";

        public static IEnumerable<string> GetAllRoleNames()
        {
            yield return ADMINISTRATOR_ROLE;
            yield return READ_ALL_SCOPES_ROLE;
            yield return CREATE_SCOPE_ROLE;
            yield return CREATE_GOAL_ROLE;
            yield return REMOVE_GOAL_ROLE;
        }
    }
}
