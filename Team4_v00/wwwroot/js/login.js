$(document).ready(function () {
    $("#loginBtn").click(function () {
        var uname = $("#uname").val();
        var pwd = $("#pwd").val();
        if (uname.length === 0 || pwd.length === 0) {
            return;
        }

        $("#hashPwd").val(CryptoJS.SHA256(pwd).toString());
        $("#pwd").val("");

        $("#loginForm").submit();
    });
});