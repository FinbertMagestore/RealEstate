
function showChangePass() {
    document.getElementById("changePass").style.display = "inherit";
    $("#showChangePass").addClass("hide");
}
document.getElementById("ConfirmPassword").onkeyup = function () {
    ConfirmPassword();
};
function ConfirmPassword() {
    var UserPassword = document.getElementById("Password").value;
    var ConfirmUserPassword = document.getElementById("ConfirmPassword").value;
    if (UserPassword != null && UserPassword != "" && ConfirmPassword != null && ConfirmPassword != "") {
        if (UserPassword != ConfirmUserPassword) {
            document.getElementById("errorConfirm").style.display = "inherit";
        } else {
            document.getElementById("errorConfirm").style.display = "none";
        }
    }
}