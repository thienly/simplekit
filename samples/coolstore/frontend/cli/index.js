const chalk = require('chalk');
const clear = require('clear');
const figlet = require('figlet');
const inquirer  = require('./lib/inquirer');


clear();

console.log(
   chalk.yellow(
     figlet.textSync('COOL STORE', { horizontalLayout: 'full' })
   )
);
// const run = async () => {
//     const credentials = await inquirer.askCredentials();
//     console.log(credentials);
//   };
  
// run();


var AuthenticationClient = require('auth0').AuthenticationClient;
var authClient = new AuthenticationClient({
  domain: 'dev-p7-5odcq.auth0.com',
  clientId: 'ztBe2PxPGzup3ZluUax9Gn5gGexJ9mZk'
});

var data = {
    username: 'clitest@gmail.com',
    password: 'new',
    realm: 'Username-Password-Authentication',
    scope: 'openid profile email address phone'
  };
  
  authClient.oauth.passwordGrant(data, function (err, userData) {
    if (err) {
      // Handle error.
      console.log(`${err} Error`);
    }
  
    console.log(userData);
});