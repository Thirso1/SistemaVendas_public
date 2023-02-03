$(document).ready(function () {
    //chama a library que faz busca por nome no select
    $('#selectClienteVenda').select2();
    $('#selectProdutoVenda').select2();

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $(".btn-danger").click(function (e) {
        var resultado = confirm("Tem certeza que deseja realizar esta operação?");

        if (resultado == false) {
            e.preventDefault();
        }
    });

    $("#id_receber_venda").click(function (e) {
        var resultado = confirm("Confirma o Recebimento?");

        if (resultado == false) {
            e.preventDefault();
        }
    });

    $('.dinheiro').mask('000.000.000.000.000,00', { reverse: true });
    $('.qtdePeso').mask('000.000.000.000.000,000', { reverse: true });

    AJAXUploadImagemProduto();
    CategoriaSlug();
    AJAXBuscarCliente();
    AJAXBuscarProduto();
    MudaValorVarejoAtacado();
    unfocusQtde();
    pressEnterQtde();

    $("#inserirItem").click(function () {
        var estoque = parseFloat($('#qtde_estoque').val());
        var qtde_pedida = parseFloat($('#qtde_item').val());


        if (qtde_pedida > 0 && qtde_pedida > estoque) {
            alert("Estoque indisponível!")
        }
        else if (qtde_pedida > 0 && qtde_pedida <= estoque) {
            insereItemGrid();
            limpaCampoInsereItemGrid();
            calculaTotal();
            calculaCustoTotal();
        }

    });

    $("#finalizarVenda").click(function () {
        var idCliente = $('#selectClienteVenda').find(":selected").val();
        if (idCliente != "") {
            var resultado = confirm("'Finalizar' a Venda?");

            if (resultado == false) {
                e.preventDefault();
            }
            else {
                var itens = ItensPedido();
                var itensJson = JSON.stringify(itens);
                AJAXEnviaItensVenda(itensJson);
            }
        }
        else {
            alert("selecione o cliente");
        }
    });

    $("#cancelarVenda").click(function () {
        var id_venda = $("#numVenda").val();

        window.location.href = '/Colaborador/Venda/Excluir/' + id_venda;
    });

    $("#suspenderVenda").click(function (e) {

        var totalVenda = calculaTotal();
        if (totalVenda == "0" || totalVenda == "0,00" || totalVenda == "0.00" || totalVenda == "") {
            alert("Insira pelo menos um Item para poder suspender!");
            e.preventDefault();
            $("#ValorTotal").focus();
        }
        else {
            var resultado = confirm("Deseja 'Suspender' a Venda?");

            if (resultado == false) {
                e.preventDefault();
            }
            else {
                $("#id_status_venda").val("Suspensa");
                var itens = ItensPedido();
                var itensJson = JSON.stringify(itens);
                AJAXSuspendeItensVenda(itensJson);
            }
            //window.location.href = '/www.google.com.br';
        }
    });

    calculaTotal();
    configuraLinksPaginacao();
    links_painel();
});


function links_painel() {
    var location = window.location.href;

    var splitlocation = location.split('/');
    var url = splitlocation[0] + "/" + splitlocation[1] + "/" + splitlocation[2] + "/" + splitlocation[3];

    $("#link_card_vendas_prazo").attr("href", url + "/Venda/Index?pagina=1&selectClienteVenda=&dataInicio=&dataFim=&statusVenda=NaoPago");

}

function buscarVendas() {
    var location = window.location.href;

        var idCliente = $('#selectClienteVenda').find(":selected").val();
        var idStatus = $('#statusVenda').find(":selected").val();
        var dataInicio = $('#dataInicio').val();
        var dataFim = $('#dataFim').val();
        const locationArray = location.split("");
        var splitlocation = location.split('/');

    if (splitlocation[4] == "Venda") {

        if (locationArray[8] == "w") {
            if (locationArray.length > 61) {
                var link = "Index?pagina=1&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                window.location.href = link;
            }
            else {
                var link = "Venda/Index?pagina=1&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                window.location.href = link;
            }
        }
        else {
            if (locationArray.length > 61) {
                var link = "Index?pagina=1&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                window.location.href = link;
            }
            else {
                var link = "Venda/Index?pagina=1&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                window.location.href = link;
            }
        }
    }
}

//essa função é para inserrir os parametros na query da paginação
function configuraLinksPaginacao() {
    $('.PagedList-skipToPrevious').remove();
    var idCliente = $('#selectClienteVenda').find(":selected").val();
    var idStatus = $('#statusVenda').find(":selected").val();
    var dataInicio = $('#dataInicio').val();
    var dataFim = $('#dataFim').val();

    var location = window.location.href;
    var locationArray = location.split('');
    var splitlocation = location.split('/');

    //verifica se está no controlador "Venda"
    if (splitlocation[4] == "Venda") {
        //alert(splitlocation[4]);
        //se aparece o "www"
        if (locationArray[8] == "w") {
                //"maior que 61
            if (locationArray.length > 61) {
                $('.pagination li').each(function (index, element) {
                    var pag = index + 1;
                    var link = "Index?pagina=" + pag + "&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                    $(this).find("a").attr("href", link);

                });
            }
                //"menor ou igual que 61
            else {
                $('.pagination li').each(function (index, element) {
                    var pag = index + 1;
                    var link = "Venda/Index?pagina=" + pag + "&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                    $(this).find("a").attr("href", link);
                });
            }
        }
         //se não aparece o "www"
        else {

            if (locationArray.length > 61) {
                $('.pagination li').each(function (index, element) {
                    var pag = index + 1;
                    var link = "Index?pagina=" + pag + "&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                    $(this).find("a").attr("href", link);

                });
                //alert("maior que 61");
            }
            else {
                $('.pagination li').each(function (index, element) {
                    var pag = index + 1;
                    var link = "Venda/Index?pagina=" + pag + "&selectClienteVenda=" + idCliente + "&dataInicio=" + dataInicio + "&dataFim=" + dataFim + "&statusVenda=" + idStatus;
                    $(this).find("a").attr("href", link);
                });
            }
        }
    }        
}

$(document).on('click', '.excluir_item_venda', function () {

    //var id_produto = $(this).parent().parent().find(".id-produto").text();

    var tr = $(this).closest('tr');
    tr.fadeOut(300, function () {
        tr.remove();
        calculaTotal();
    });    
});


function ItensPedido() {
    // Percorrer todas as linhas do corpo da tabela
        var itensPedidos = [];
    $('#tabelaPedido tbody tr').each(function () {
        // Recuperar todas as colunas da linha percorida
        var colunas = $(this).children();
        // Criar objeto para armazenar os dados
        var item = {
            'ProdutoId': parseInt($(colunas[0]).text(), 10), // valor da coluna Produto
            'Nome': $(colunas[1]).text(), // valor da coluna Produto
            'ValorUnitario': parseFloat($(colunas[2]).text()), // Valor da coluna Quantidade
            'Desconto': parseFloat($(colunas[3]).text()), // Valor da coluna Quantidade
            'Qtde': parseFloat($(colunas[4]).text()), // Valor da coluna Quantidade
            'TotalItem': parseFloat($(colunas[5]).text()), // Valor da coluna Quantidade
            'VendaId': parseInt($(colunas[6]).text()) // Valor da coluna Quantidade
        };
        // Adicionar o objeto pedido no array
        itensPedidos.push(item);
    });

    return itensPedidos;
}


function calculaTotal() {

    var valorCalculado = 0;

    $(".valor-calculado").each(function () {
        valorCalculado += parseFloat($(this).text());
    });
    $("#lblTotal").text(valorCalculado.toFixed(2));
    $("#valorTotal").val(valorCalculado.toFixed(2));
    return valorCalculado.toFixed(2);
}

function calculaCustoTotal() {

    var valorCalculado = 0;

    $(".valor_custo_calculado").each(function () {
        valorCalculado += parseFloat($(this).text());
    });
    $("#custoTotal").val(valorCalculado.toFixed(2));
    return valorCalculado.toFixed(2);
}

function limpaCampoInsereItemGrid() {
    $('#id_produto').val("");
    $('#selectProdutoVenda').prop("selectedIndex", 0).val();
    $('#val_uni').val("");
    $('#qtde_item').val("1");
    $('#total_item').val("");    
}

//essa função le os dados da tabela de itens e retorna um Json com todos os itens da venda
function importTable() {
    var myRows = [];
    var headersText = [];
    var $headers = $("th");

    // Loop through grabbing everything
    var $rows = $("tbody tr").each(function (index) {
        $cells = $(this).find("td");
        myRows[index] = {};

        $cells.each(function (cellIndex) {
            // Set the header text
            if (headersText[cellIndex] === undefined) {
                headersText[cellIndex] = $($headers[cellIndex]).text();
            }
            // Update the row object with the header/cell combo
            myRows[index][headersText[cellIndex]] = $(this).text();
        });
    });

    // Let's put this in the object like you want and convert to JSON (Note: jQuery will also do this for you on the Ajax request)

    var myObj2 = JSON.stringify(myRows);

    AJAXEnviaItensVenda(myObj2, $("#numVenda").val())

    return JSON.stringify(myRows);
}

function formataNumJson(jsonItens) {
    for (var i = 0; i < jsonItens.length; i++) {
        alert(jsonItens[i].Nome);
        
    } 
    return jsonItens;
}


//todo cadastro itens venda
function AJAXEnviaItensVenda(jsonItens) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/api/ItemVendas",
            data: jsonItens,
            error: function () {
                alert("Opss! Contate o suporte!");

            },
            success: function () {
            }
        });
}

//todo cadastro itens venda
function AJAXSuspendeItensVenda(jsonItens) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/api/ItemVendas/suspensos",
        data: jsonItens,
        error: function () {
            alert("Opss! Contate o suporte!");

        },
        success: function () {
        }
    });
}


function CategoriaSlug() {
    if ($("#form-categoria").length > 0) {
        $("input[name=Nome]").keyup(function () {
            $("input[name=Slug]").val(convertToSlug(  $(this).val() ) );
        });
    }
}
function AJAXUploadImagemProduto() {
    $(".img-upload").click(function () {
        $(this).parent().parent().find(".input-file").click();
    });
    $(".btn-imagem-excluir").click(function () {
        var CampoHidden = $(this).parent().find("input[name=imagem]");
        var Imagem = $(this).parent().find(".img-upload");
        var BtnExcluir = $(this).parent().find(".btn-imagem-excluir");
        var InputFile = $(this).parent().find(".input-file");

        $.ajax({
            type: "GET",
            url: "/Colaborador/Imagem/Deletar?caminho=" + CampoHidden.val(),
            error: function () {

            },
            success: function () {
                Imagem.attr("src", "/img/no-image.svg");
                BtnExcluir.addClass("btn-ocultar");
                CampoHidden.val("");
                InputFile.val("");
            }
        });
    });

    $(".input-file").change(function () {

        var ArquivoEnviado = $(this)[0].files[0].name;
        var EPermitidoUpload = true;

        $("input[name=imagem]").each(function () {
            if ($(this).val().length > 0) {
                var NomeDaImagem = $(this).val().split("/")[3];

                if (NomeDaImagem == ArquivoEnviado) {
                    alert(`Não é permitido enviar imagens com o mesmo nome, renomeia a imagem e envie novamente! (${NomeDaImagem})`);
                    EPermitidoUpload = false;
                }
            }
        });

        if (!EPermitidoUpload) return;

        //Formulário de dados via JavaScript
        var Binario = $(this)[0].files[0];
        var Formulario = new FormData();
        Formulario.append("file", Binario);

        var CampoHidden = $(this).parent().find("input[name=imagem]");
        var Imagem = $(this).parent().find(".img-upload");
        var BtnExcluir = $(this).parent().find(".btn-imagem-excluir");

        Imagem.attr("src", "/img/loading.gif");
        Imagem.addClass("thumb");
                
        $.ajax({
            type: "POST",
            url: "/Colaborador/Imagem/Armazenar",
            data: Formulario,
            contentType: false,
            processData: false,
            error: function () {
                alert("Erro no envio do arquivo!");
                Imagem.attr("src", "/img/no-image.svg");
                Imagem.removeClass("thumb");
            },
            success: function (data) {
                var caminho = data.caminho;
                Imagem.attr("src", caminho);
                Imagem.removeClass("thumb");
                CampoHidden.val(caminho);
                BtnExcluir.removeClass("btn-ocultar");
            }
        });
    });
}

function LimpaCamposCliente() {
    $("#nomeCliente").text("");
    $("#cpf_cnpj").text("");
    $("#enderecoCliente").text("");
    $("#bairroCliente").text("");
    $("#Complemento").text("");
    $("#cidadeCliente").text("");
    $("#cepCliente").text("");
    $("#telefone1Cliente").text("");
    $("#telefone2Cliente").text("");
    $("#emailCliente").text("");
    $("#ufCliente").text("");
}



function AJAXBuscarCliente() {
    $('#selectClienteVenda').change(function () {
        LimpaCamposCliente();
        //OcultarMensagemDeErro();

            $.ajax({
                type: "GET",
                url: "/Colaborador/Cliente/BuscaClienteAjax/" + $(this).val(),
                dataType: "json",
                error: function (data) {
                    MostrarMensagemDeErro("Opps! Não foi possível carregar os dados do Cliente!");
                },
                success: function (data) {
                    if (data.erro == undefined) {
                        $("#nomeCliente").append('<b>Cliente: </b>' + data.Nome);
                        $("#cpf_cnpj").append('<b>Cpf/Cnpj: </b>' + data.CPF_CNPJ);
                        $("#enderecoCliente").append('<b>Endereço: </b>' + data.Endereco + ", " + data.Numero);
                        $("#bairroCliente").append('<b>Bairro: </b>' + data.Bairro);
                        $("#Complemento").text(data.Complemento);
                        $("#cidadeCliente").append('<b>Cidade: </b>' + data.Cidade);
                        $("#cepCliente").append('<b>Cep: </b>' +data.CEP);
                        $("#telefone1Cliente").append('<b>Contato: </b>' + data.Telefone);
                        $("#telefone2Cliente").text(data.Telefone_2);
                        $("#emailCliente").append('<b>e-mail: </b>' + data.Email);
                        $("#ufCliente").append('<b>UF: </b>' +data.Estado);
                    } else {
                        MostrarMensagemDeErro("Não foi possível carregar os dados do Cliente");
                    }

                }
            });
        
    })
}

function AJAXBuscarProduto() {
    $('#selectProdutoVenda').change(function () {
        //OcultarMensagemDeErro();

        $.ajax({
            type: "GET",
            url: "/Colaborador/Produto/BuscaProdutoAjax/" + $(this).val(),
            dataType: "json",
            error: function (data) {
                MostrarMensagemDeErro("Opps! tivemos um erro!");
            },
            success: function (data) {
                if (data.erro == undefined) {
                    $("#id_produto").val(data.Id);
                    $("#custo_item").val(data.ValorCusto.toFixed(2));
                    $("#custo_total_item").val(data.ValorCusto.toFixed(2));
                    $("#val_varejo").val(data.ValorVarejo.toFixed(2));
                    $("#val_atacado").val(data.ValorAtacado.toFixed(2));
                    $("#val_uni").val(data.ValorVarejo.toFixed(2));
                    $("#qtde_estoque").val(data.Estoque.toFixed(3));
                    $('#total_item').val(data.ValorVarejo.toFixed(2));
                } else {
                    MostrarMensagemDeErro("Contate o supoerte!");
                }
            }
        });
    })
}

//colcula o valor total qdo muda o select 
function MudaValorVarejoAtacado() {
    $('#selectTipoPreco').change(function () {
        var varejo = $('#val_varejo').val();
        var atacado = parseFloat($('#val_atacado').val().toFixed(2));
        if ($('#selectTipoPreco').val() == "Varejo") {
            $('#val_uni').val(varejo);
        }
        else {
            $('#val_uni').val(atacado);
        }
        $('#total_item').val(calculaTotalItem());
    })
}

function calculaTotalItem() {
    var unitario = parseFloat($('#val_uni').val());
    var qtde = parseFloat($('#qtde_item').val());
    return (unitario * qtde).toFixed(2);   
}

function calculaCustoItem() {
    var unitario = parseFloat($('#custo_item').val());
    var qtde = parseFloat($('#qtde_item').val());
    return (unitario * qtde).toFixed(2);
}

//colcula o valor total qdo qtde perde o foco 
function unfocusQtde() {
    $('#qtde_item').focusout(function () {
        var estoque =  parseFloat($('#qtde_estoque').val());
        var qtde_pedida = parseFloat($('#qtde_item').val());

        if (qtde_pedida > estoque) {
            alert("Estoque indisponível!")
        }
        else {
            var total = calculaTotalItem();
            var total_custo = calculaCustoItem();
            $('#total_item').val(total);
            $('#custo_total_item').val(total_custo);
        }
        
    })
}

function pressEnterQtde() {
    $('#qtde_item').keypress(function (event) {
        if (event.which == 13) {
            var total = calculaTotalItem();
            $('#total_item').val(total);
        }
    })
}
//hidden
function insereItemGrid() {
    $('#bodyTable').append(
        '<tr>'+
        '<td hidden class= "id-produto" > ' + $('#id_produto').val() + '</td > ' +
        '<td> ' + $('#selectProdutoVenda').find(":selected").text() + '</td>' +
        '<td> ' + $('#val_uni').val() + '</td > '+
        '<td hidden>0</td>'+
        '<td>'+$('#qtde_item').val() + '</td>'+
        '<td class= "valor-calculado" > ' + $('#total_item').val() +'</td > '+
        '<td hidden > ' + $('#numVenda').val() + '< td >' +
        '< td ><a class="excluir_item_venda btn">Excluir</a></td>'+
        '<td hidden>' + $('#custo_item').val() + '</td>'+
        '<td hidden class= "valor_custo_calculado">' + $('#custo_total_item').val() + '</td>'+
        '</tr>'
    );
}

function editarItemVenda(id_item) {


}







function convertToSlug(Text) {
    return Text
        .toLowerCase()
        .replace(/ /g, '-')
        .replace(/[^\w-]+/g, '')
        ;
}







