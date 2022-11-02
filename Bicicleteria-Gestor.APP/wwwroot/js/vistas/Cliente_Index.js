const MODELO_BASE = {
    idCliente: 0,
    documento: "",
    nombreCompleto: "",
    ciudad: "",
    direccion: "",
    deudor: 1
}

let tablaData;
let filaSeleccionada;

//-----------Modal Vacio ---------------


function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idCliente)
    $("#txtNumeroDocumento").val(modelo.documento)
    $("#txtNombreCompleto").val(modelo.nombreCompleto)
    $("#txtCiudad").val(modelo.ciudad)
    $("#txtDireccion").val(modelo.direccion)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboEstado").val(modelo.deudor)

    $("#modalData").modal("show")
}

//-----------Nuevo Cliente ------------

$("#btnNuevo").click(function () {
    mostrarModal();
})

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Cliente/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCliente", "visible": false, "searchable": false },
            { "data": "documento" },
            { "data": "nombreCompleto" },
            { "data": "ciudad" },
            { "data": "direccion" },
            {
                "data": "deudor", render: function (data) {
                    if (data == 0) {
                        return '<span class="badge badge-info">No Deudor</span>';
                    } else {
                        return '<span class="badge badge-danger">Deudor</span>';
                    }
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "90px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Clientes',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

//-----------Guardar -----------

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "");

    if (inputs_sin_valor.length > 0) {

        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"`).focus()
        return;
    }

    //Modelo que se envia para crear

    const modelo = structuredClone(MODELO_BASE);

    modelo["idCliente"] = parseInt($("#txtId").val())
    modelo["documento"] = $("#txtNumeroDocumento").val()
    modelo["nombreCompleto"] = $("#txtNombreCompleto").val()
    modelo["ciudad"] = $("#txtCiudad").val()
    modelo["direccion"] = $("#txtDireccion").val()
    modelo["deudor"] = $("#cboEstado").val()

    //FormData va a ser lo que le enviamos a nuestro metodo del backend (lo recibe como FromForm)
    const formData = new FormData();

    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCliente == 0) {

        fetch("/Cliente/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "El Cliente fue creado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Cliente/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide");
                    swal("Listo!", "El Cliente fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
});

//--------Editar---------

$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();

    mostrarModal(data);
})

//-----------Eliminar ---------------


$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar el Cliente "${data.nombreCompleto}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, Eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Cliente/Eliminar?idCliente=${data.idCliente}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {

                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();

                            $("#modalData").modal("hide");
                            swal("Listo!", "El Cliente fue eliminado", "success")
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })
            }
        }

    )
})