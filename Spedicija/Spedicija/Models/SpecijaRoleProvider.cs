﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data.Entity;

namespace Spedicija.Models
{
    public class SpecijaRoleProvider : RoleProvider
    {

        public override bool IsUserInRole(string username, string roleName)
        {
            int idrola = Convert.ToInt32(roleName);
            uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

            Korisnik korisnik = db.Korisnik.Single(k => k.KorisnickoIme.Equals(username));

            return (korisnik.Role.Equals(roleName));


            // throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get;
            set;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            String[] niz = new String[2] { "1", "2" };
            return niz;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

    }
}