<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="XDomainProxy._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="http://apps.bdimg.com/libs/jquery/2.1.4/jquery.min.js"></script>

</head>
<body>
    <button>点我</button>

    <script>
        $(document).ready(function () {
            $("button").click(function () {
                console.log('ok');
                $.post("/api/base/Banji", null,
                function (data, status) {
                    console.log(data);
                    alert(status);
                    //alert("数据: \n" + data + "\n状态: " + status);
                });
            });
        });

        
    </script>

</body>
</html>
