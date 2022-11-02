//------- datos del desplegable

$(document).ready(function () {

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {

                return {
                    results: data.map((item) => (
                        {
                            id: item.idProducto,
                            text: item.descripcion,
                            marca: item.marca,
                            categoria:item.nombreCategoria,
                            urlImagen: item.urlImagen,
                            precio: parseFloat(item.precio)
                        }
                    ))
                };
            }
        },
        language: "es",
        placeholder: 'Buscar Producto...',
        minimumInputLength: 1,
        templateResult: formatoResultado
    });
})

//-------- diseño del desplegable que aparece cuando busco 

function formatoResultado(data) {

    if (data.loading)
        return data.text;

    var contenedor = $(
        `<table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
                </td>
                <td>
                    <p style="font-weight: bolder;margin:2px">${data.marca}</p> 
                    <p style="margin:2px">${data.text}</p> 
                </td>
            </tr>
         <table>`
    );
    return contenedor;
}

//--------- poner el cursor donde tengo que escribir para realizar la busqueda
$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let productosParaVentas = [];

//-------------Ingresar productos a la lista de venta

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;
    let producto_encontrado = productosParaVentas.filter(p => p.idProducto == data.id);

    if (producto_encontrado.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado")
        return false
    }
    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type:"input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese Cantidad"
    },
        function (valor) {

            if (valor === false) return false

            if (valor === "") {
                toastr.warning("", "Necesita ingresar la cantidad")
                return false;
            }

            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ingresar un valor numerico")
                return false;
            }

            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total:(parseFloat(valor) * data.precio).toString()
            }
            productosParaVentas.push(producto);

            mostrarProductos_Precios();
            $("#cboBuscarProducto").val("").trigger("change")
            swal.close()
        }
    )
})

//--------- Mostrar detalles

function mostrarProductos_Precios() {

    let total = 0;

    $("#tbProducto tbody").html("")

    productosParaVentas.forEach((item) => {
        total = total + parseFloat(item.total)
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto",item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    $("#txtTotal").val(total);
}

//-----------Eliminar producto de venta-------------

$(document).on("click", "button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto") //obtener el idProducto que se desea eliminar

    productosParaVentas = productosParaVentas.filter(p => p.idProducto != _idProducto);

    mostrarProductos_Precios();
})


//------------ terminar la venta ------------
$("#btnTerminarVenta").click(function () {
    if (productosParaVentas.length < 1) {
        toastr.warning("", "Debe ingresar productos")
        return;
    }
    //const vmDetalleVenta = productosParaVentas;

    const venta = {
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        total: $("#txtTotal").val(),
        detalleVenta: productosParaVentas
    }

    $("#btnTerminarVenta").LoadingOverlay("show");

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: {"Content-Type": "application/json; charset=utf-8"},
        body: JSON.stringify(venta)
        })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosParaVentas = [];
                mostrarProductos_Precios();
                $("#txtDocumentoCliente").val("")
                $("#txtNombreCliente").val("")

                swal("Registrado!", `Numero de Venta: ${responseJson.objeto.numeroVenta}`, "success")
            } else {
                swal("Lo sentimos", "No se pudo registrar la venta", "error")
            }
        })
})




