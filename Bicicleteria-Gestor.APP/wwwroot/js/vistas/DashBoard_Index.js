$(document).ready(function () {
    $("div.container-fluid").LoadingOverlay("show");

    fetch("/DashBoard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                let d = responseJson.objeto;

                //mostrar datos tarjetas
                $("#totalVenta").text(d.totalVentas)
                $("#totalIngresos").text(d.totalIngresos)
                $("#totalProductos").text(d.totalProductos)

                //obtener datos grafico barras
                let barchart_labels;
                let barchart_data;

                if (d.ventasUltimaSemana.length > 0) {
                    barchart_labels = d.ventasUltimaSemana.map((item) => { return item.fecha });
                    barchart_data = d.ventasUltimaSemana.map((item) => { return item.total });
                } else {
                    barchart_labels = ["Sin resultados"];
                    barchart_data = [0];

                }

                //Grafico de Barras
                let controlVenta = document.getElementById("charVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#6D6B6A",
                            hoverBackgroundColor: "#979594",
                            borderColor: "#6D6B6A",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });
            }
        })
})