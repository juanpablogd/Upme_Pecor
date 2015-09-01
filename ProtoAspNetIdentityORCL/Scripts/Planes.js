$(document).ready(function () {
    console.log("ready document PLAN");
      
    $('#FECHA_INICIO').pickadate({
        format: 'yyyy-mm-dd',
    })
    $('#FECHA_FINAL').pickadate({
        format: 'yyyy-mm-dd',
    })
});