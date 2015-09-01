$(document).ready(function () {

    console.log("ready document Priorización");

    $("#btn_submit").click(function () {
        console.log("Click");
        $("#form_Calcular").submit();
    });

    $("form").submit(function (e) {
        console.log("Valor plan" + $("#VID_PLAN").val());
        if ($("#VID_PLAN").val() == "") {
            alert("Debe seleccionar un Plan");
            $("#VID_PLAN").focus();
            e.preventDefault();
            return false;
        }
    });

});