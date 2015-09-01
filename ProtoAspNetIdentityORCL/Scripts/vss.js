$(document).ready(function () {
    console.log("ready document VSS");
    //$('#INI_EJEC_INV').datepicker({});

        
    $('#COD_DPTO').change(function () {

        var input = $("<input>")
               .attr("type", "hidden")
               .attr("name", "chgSitio").val("true");
        $('form').append($(input));
        $("form").submit()
    });
    $('#COD_MPIO').change(function () {
        var input = $("<input>")
               .attr("type", "hidden")
               .attr("name", "chgSitio").val("true");
        $('form').append($(input));
        $("form").submit()
    });

    $("#DEMANDA_ANUAL").blur(function () {
        console.log(validarEntero($("#DEMANDA_ANUAL").val()));
        var valor = validarEntero($("#DEMANDA_ANUAL").val());
        $("#DEMANDA_ANUAL").val(valor);
    });

    $("#VSS_BENEF").blur(function () {
        console.log(validarEntero($("#VSS_BENEF").val()));
        var valor = validarEntero($("#VSS_BENEF").val());
        $("#VSS_BENEF").val(valor);
    });

    $('input[type="text"]').keydown(function (e) { //(event.keyCode< 48 || event.keyCode > 57
        console.log(e.keyCode);
        // Allow: backspace, delete, tab, escape, enter and .(190 NO) 
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode == 65 && ( e.ctrlKey === true || e.metaKey === true ) ) || 
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
        
    });

    function validarEntero(valor){ 
        //intento convertir a entero. 
        //si era un entero no le afecta, si no lo era lo intenta convertir 
        valor = parseInt(valor) 
        //Compruebo si es un valor numérico 
        if (isNaN(valor)) { 
            //entonces (no es numero) devuelvo el valor cadena vacia 
        }else{ 
            //En caso contrario (Si era un número) devuelvo el valor 
            return valor 
        } 
    }
    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
    }
});