$(document).ready(function () {
    $("#loginBtn").click(function () {

        var uname = $("#uname").val();
        var pwd = $("#pwd").val();
        var utf8arr = CryptoJS.enc.Utf8.parse(pwd);
        var hash = CryptoJS.SHA256(utf8arr);
        var stringPwd = hash.toString(CryptoJS.enc.Base64);

        if (uname.length === 0 || pwd.length === 0) {
            return;
        }

        $("#hashPwd").val(stringPwd);
        $("#pwd").val("");

        $("#loginForm").submit();
    });
});