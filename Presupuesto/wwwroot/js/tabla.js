
$(document).ready(function () {
    $('#tablaTipoCuentas').DataTable({
        "serverSide": true,
        "ajax": {
            "url": "/TiposCuentas/GetTipoCuentas",
            "type": "GET",
            "data": function (data) {
                data.draw = data.draw || 1;
            }
        },
        "columns": [
            { "data": "id" },
            { "data": "nombre" },
            {
                "data": null,
                "render": function (data, type, row) {
                    return '<button onclick="editar(' + row.id + ')" class="btn btn-primary">Editar</button>' +
                        '<button onclick="eliminar(' + row.id + ')" class="btn btn-danger">Eliminar</button>';
                }
            }
        ]
    });
});


function editar(id) {
    var url = `/TiposCuentas/Editar/${id}`;
    window.location.href = url;
}


function eliminar(id) {
    fetch(`/TiposCuentas/BorrarTipoCuenta?id=${id}`,{method:'POST'})
        .then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                return Promise.reject('Error en la solicitud POST: ${ response.status }');
            }
        })
        .catch(error => console.error(error));
}

