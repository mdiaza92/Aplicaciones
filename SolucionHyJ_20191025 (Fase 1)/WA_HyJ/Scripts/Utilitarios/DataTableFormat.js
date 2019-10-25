$(document).ready(function () {

    var tableDT = $('table#DataTable').DataTable({
        "lengthChange": false,
        "scrollY": "600px",
        "colReorder": false,
        "fixedHeader": true,
        "bAutoWidth": true,
        "autoWidth": true,
        "scrollCollapse": true,
        "stateSave": true,
        "responsive": {
            "details": {
                "renderer": function (api, rowIdx, columns) {
                    var data = $.map(columns, function (col, i) {
                        return col.hidden ?
                            '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                            '<td><b>' + col.title + '</b></td> ' +
                            '<td>' + col.data + '</td>' +
                            '</tr>' :
                            '';
                    }).join('');

                    return data ?
                        $('<table/>').append(data) :
                        false;
                }
            },
            "type": "column"
        },
        "dom": 'Bfrtip',
        "lengthMenu": [
            [10, 25, 50, -1],
            ['10 filas', '25 filas', '50 filas', 'Todos']
        ],
        "language": {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            },
            "buttons": {
                "copyTitle": 'Elementos copiados',
                "copySuccess": {
                    1: "Se copió un elemento",
                    _: "Se copió %d filas"
                },
                "pageLength": {
                    '-1': 'Mostrar todos <i class="fa fa-filter"></i>',
                    _: 'Mostrar %d filas <i class="fa fa-filter"></i>'
                }
            }
        },
        "buttons": [
            {
                extend: 'colvis',
                text: 'Columnas visibles <i class="fa fa-bars"></i>',
                columns: ':not(:last-child)',
                postfixButtons: [{
                    extend: 'colvisRestore',
                    text: 'Restaurar columnas'
                }],
                collectionLayout: 'fixed two-column'
            },
            {
                extend: 'pageLength'
            }
        ],
        "columnDefs": [
            {
                "targets": 'lastcolumn',
                "searchable": false,
                "orderable": false
            }
        ]
    });

    tableDT.columns.adjust().draw();
});