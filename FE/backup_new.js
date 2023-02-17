const { exec } = require("child_process");
var Prompt = require("prompt-base");
const fs = require("fs");
const path = require("path");
const { NodeSSH } = require("node-ssh");
const zipFolder = require("zip-a-folder");
const { FILE } = require("dns");
const password = "Vhec#CanTho!";

const ssh = new NodeSSH();
ssh.connect({
    host: "194.233.88.203",
    username: "root",
    port: 22,
    password,
    tryKeyboard: true,
});

var myArgs = process.argv.slice(2);
console.log(myArgs);
const comCd = "cd ../FE";
const comInstall = "npm install";
const comBuildFE = "npm run build:" + myArgs[0];

var FE_ZIP_NAME = "FE.zip";
var BE_ZIP_NAME = "BE.zip";

fePathBuild = "../FE/dist";
bePathBuild = "../BE/bin";

let FE_PATH_SERVER;
let BE_PATH_SERVER;
//DEV
if (myArgs[0] === "dev") {
    FE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/" + myArgs[0] + "/FE/";
    BE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/" + myArgs[0] + "/BE/";
}

//Stage
if (myArgs[0] === "stage") {
    FE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/ims/FE/";
    BE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/ims/BE/";
}

//Production
if (myArgs[0] === "production") {
    FE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/" + myArgs[0] + "/FE/";
    BE_PATH_SERVER = "/home/nhmen/Workspace/VHEC/" + myArgs[0] + "/BE/";
}

new Prompt("Are you want to deploy to: " + myArgs[0] + "? (Y/n)")
    .run()
    .then(async function(answer) {
        if (!answer || answer.toLowerCase() !== "y") {
            return;
        } else {
            //check folder build
            if (fs.existsSync("build")) {
                exec("rmdir build /S /Q", (error, stdout, stderr) => {
                    if (error) {
                        console.log(`error: ${error.message}`);
                        return;
                    }
                    if (stderr) {
                        console.log(`stderr: ${stderr}`);
                        return;
                    }
                    exec("mkdir build", (error, stdout, stderr) => {
                        if (error) {
                            console.log(`error: ${error.message}`);
                            return;
                        }
                        if (stderr) {
                            console.log(`stderr: ${stderr}`);
                            return;
                        }
                    });
                });
            }
            //deploy
            await deploy();
        }
    });

function deploy() {
    new Prompt("Are you want to deploy to " + myArgs[0] + "? (FE / BE / ALL?)")
        .run()
        .then(async function(answer) {
            if (answer.toLowerCase() == "all") {
                runDeployFE();
                runDeployBE();
            } else if (answer.toLowerCase() == "fe") {
                await runDeployFE();
            } else if (answer.toLowerCase() == "be") {
                runDeployBE();
            } else {
                return;
            }

            // await uploadFile();
        });
}

async function runDeployFE() {
    console.time("Time");
    // install source F.E
    console.log(">>> npm install ....");

    await exec(`${comCd} && ${comInstall}`, async(error, stdout, stderr) => {
        if (error) {
            console.log(`error: ${error.message}`);
            return;
        }
        if (stderr) {
            console.log(`stderr: ${stderr}`);
            return;
        }
        console.log(`stdout: ${stdout}`);
    });
    console.log(`====>Install OK`);
    return;
}

function runDeployBE() {}

async function zipFolderToFile(path, fileName) {
    await zipFolder.zip(path, "./build/" + fileName);
}

/**
 * Function Upload File to VPS
 */
async function uploadFile() {
    ssh
        .execCommand("rm -rf " + FE_PATH_SERVER)
        .then(function(result) {
        })
        .then(function(result) {
            ssh
                .putFile(
                    "./build/" + FE_ZIP_NAME,
                    FE_PATH_SERVER + FE_ZIP_NAME
                )
        })
        .then(function(result) {});
}