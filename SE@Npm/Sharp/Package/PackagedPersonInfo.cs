// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    /// <summary>
    /// Provides personal information inside of a package info instance
    /// </summary>
    public class PersonInfo
    {
        string name;
        /// <summary>
        /// The name of this person
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        string email;
        /// <summary>
        /// An email address this person provided
        /// </summary>
        public string Email
        {
            get { return email; }
        }

        Uri home;
        /// <summary>
        /// The homepage of the person
        /// </summary>
        public Uri Home
        {
            get { return home; }
        }

        /// <summary>
        /// Creates a new person info instance
        /// </summary>
        public PersonInfo(string name, string email, Uri home)
        {
            this.name = name;
            this.email = email;
            this.home = home;
        }
    }
}