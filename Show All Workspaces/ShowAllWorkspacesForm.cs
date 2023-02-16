using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Stimulsoft.Server.Connect;

namespace ShowAllWorkspaces
{
    public partial class ShowAllWorkspacesForm : Form
    {
        public ShowAllWorkspacesForm()
        {
            InitializeComponent();
        }

        #region Fields
        private static readonly string ServerAddress = "localhost:17764";
        private static readonly string ServerRestApiUrl = $"http://{ServerAddress}/1";

        private static readonly string Username = "user@name.com";
        private static readonly string Password = "password";
        #endregion

        #region Methods
        private static JObject GetResponseJson(WebRequest request)
        {
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream))
            {
                var responseString = responseReader.ReadToEnd();

                return JObject.Parse(responseString);
            }
        }
        #endregion

        #region Handlers
        private void ShowWorkspacesThroughNetApi(object sender, EventArgs e)
        {
            // Create connection to Stimulsoft Server.
            var connection = new StiServerConnection(ServerAddress);

            // Log in to the Server.
            connection.Accounts.Users.Login(Username, Password);

            // Check that logged user is Supervisor,
            // for only Supervisor can access all workspaces.
            if (!connection.Accounts.Roles.Current.IsSupervisor)
            {
                MessageBox.Show(
                   $"User \"{Username}\" is not a Supervisor and cannot access workspaces list.",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);

                return;
            }

            // Get list of workspaces.
            var workspaceNames = connection.Accounts.Workspaces.FetchAll()
                .Select(workspace => workspace.Name);

            // Show list of workspaces.
            MessageBox.Show(
                $"Available workspaces:{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, workspaceNames)}",
                "Workspaces",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Log out.
            connection.Accounts.Users.Logout();
        }

        private void ShowWorkspacesThroughRestApi(object sender, EventArgs e)
        {
            // Log in to Server and acquire session key.
            var logInUrl = $"{ServerRestApiUrl}/login";

            var logInRequest = WebRequest.Create(logInUrl);
            logInRequest.Method = "GET";
            logInRequest.Headers.Add("x-sti-UserName", Username);
            logInRequest.Headers.Add("x-sti-Password", Password);

            var logInResponse = GetResponseJson(logInRequest);

            var sessionKey = logInResponse.Value<string>("ResultSessionKey");

            // Check that logged user is Supervisor,
            // for only Supervisor can access all workspaces.
            var isLoggedInAsSupervisor = logInResponse.Value<JObject>("ResultRole").Value<bool>("IsSupervisor");

            if (!isLoggedInAsSupervisor)
            {
                MessageBox.Show(
                    $"User \"{Username}\" is not a Supervisor and cannot access workspaces list.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Get list of workspaces.
            var workspacesUrl = $"{ServerRestApiUrl}/workspaces";

            var workspacesRequest = WebRequest.Create(workspacesUrl);
            workspacesRequest.Method = "GET";
            workspacesRequest.Headers.Add("x-sti-SessionKey", sessionKey);

            var workspacesResponse = GetResponseJson(workspacesRequest);

            var workspaceNames = workspacesResponse
                .Value<JArray>("ResultWorkspaces")
                .Cast<JObject>()
                .Select(workspaceEntry => workspaceEntry.Value<string>("Name"));

            // Show list of workspaces.
            MessageBox.Show(
                $"Available workspaces:{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, workspaceNames)}",
                "Workspaces",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Log out.
            var logOutUrl = $"{ServerRestApiUrl}/logout";

            var logOutRequest = WebRequest.Create(logOutUrl);
            logOutRequest.Method = "DELETE";
            logOutRequest.Headers.Add("x-sti-SessionKey", sessionKey);

            logOutRequest.GetResponse();
        }
        #endregion
    }
}
