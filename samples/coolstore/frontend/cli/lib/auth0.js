const CLI = require('clui');
const Configstore = require('configstore');
const Spinner = CLI.Spinner;

const inquirer = require('./inquirer');
const pkg = require('../package.json');

const conf = new Configstore(pkg.name);