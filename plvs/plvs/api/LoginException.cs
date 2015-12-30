using System;

namespace Atlassian.plvs.api {
    public class LoginException : Exception {
        public LoginException(Exception e) : base("Login failed", e) { }
    }
}
