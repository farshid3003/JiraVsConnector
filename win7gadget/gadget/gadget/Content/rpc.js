rpc = function() { }

rpc.login = function(url, login, password, callback, callbackErr) {
    xmlrpc(
        url + "/rpc/xmlrpc",
        "jira1.login",
        [login, password],
        function(ret) {
            callback(ret);
        },
        function(err) {
            callbackErr(err);
        },
        function() { }
    );
}

rpc.getprojects = function(url, token, callback, callbackErr) {
    xmlrpc(
        url + "/rpc/xmlrpc",
        "jira1.getProjectsNoSchemes",
        [token],
        function(ret) {
            callback(ret);
        },
        function(err) {
            callbackErr(err);
        },
        function() {
        }
    );
}
