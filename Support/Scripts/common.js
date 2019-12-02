function Common() {
    _this = this;

    this.init = function () {
        ViewReqs(0);
        
        $("#viewReqs").click(function () {            
            var val;

            if ($("#filterReq").length == 0) {
                val = "0";
            }
            else {
                val = $("#filterReq").val();
            }

            ViewReqs(val);
        });

        $("#viewEmps").click(function () {
            var val;

            if ($("#filterEmp").length == 0) {
                val = "0";
            }
            else {
                val = $("#filterEmp").val();
            }

            ViewEmps(val);
        });

        $("#viewPars").click(function () {
            ViewPars();
        });

        $("body").on("click", ".request", function (e) {
            $(".selected").removeClass("selected");
            $(this).addClass("selected");

            $("#cancelReq").prop("disabled", false);
            $("#checkStatus").prop("disabled", false);
        });

        $("body").on("click", ".employee", function (e) {
            $(".selected").removeClass("selected");
            $(this).addClass("selected");

            $("#cancelEmp").prop("disabled", false);
        });

        $("body").on("click", ".parameter", function (e) {
            $(".selected").removeClass("selected");
            $(this).addClass("selected");

            $("#editPar").prop("disabled", false);
        });

        $("body").on("change", "#filterReq", function (e) {
            ViewReqs($("#filterReq").val());
        });

        $("body").on("change", "#filterEmp", function (e) {
            ViewEmps($("#filterEmp").val());
        });
        
        $("body").on("click", "#createReq", function (e) {
            FillModal("createRequest");
            $('#createModal').modal("show");
        });

        $("body").on("click", "#createEmp", function (e) {
            FillModal("createEmployee");
            $('#createModal').modal("show");
        });

        $("body").on("click", "#editPar", function (e) {
            FillModal("editParameter");
            $('#createModal').modal("show");            
        });

        $("body").on("click", "#cancelReq", function (e) {
            var id = $(".selected").attr('id');
            CancelRequest(id.substr(4));
        });

        $("body").on("click", "#cancelEmp", function (e) {
            var id = $(".selected").attr('id');
            CancelEmployee(id.substr(4));
        });

        $("body").on("click", "#checkStatus", function (e) {
            var id = $(".selected").attr('id');
            CheckStatusRequest(id.substr(4));
        });

        $("body").on("click", "#CancelCreate", function (e) {
            $('#createModal').modal('hide');
        });

        $("body").on("click", "#OKCreate", function (e) {
            type = $("#modalContent").attr('type');
            
            Create(type);

            $('#createModal').modal('hide');
        });

        $("body").on("click", "#infoCloseBtn", function (e) {
            $('#infoModal').modal('hide');
        });
    }
}

function FillModal(type)
{    
    $("#modalContent").attr('type', type);

    switch (type) {
        case "createRequest":
            html = '<form>' +
                        '<div class="form-group">' +
                            '<label for="exampleFormControlTextarea1">Текст заявки</label>' +
                            '<textarea id="requestText" rows="7" style="min-width: 100%"></textarea>' +
                        '</div>' +
                    '</form>'
                ;
            $("#modalContent").html(html);
            return;
        case "createEmployee":
            var html = '<form>' +
                                  '<div class="form-group row">' +
                                    '<label for="empName" class="col-sm-2 col-form-label">Name</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" class="form-control-plaintext" id="empName" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqState" class="col-sm-2 col-form-label">Title</label>' +
                                    '<div class="col-sm-10">' +
                                      '<select class="form-control-plaintext" id="empTitle" style="min-width:100%">' +                                        
                                        '<option value="0" selected>Оператор</option>' +
                                        '<option value="1">Менеджер</option>' +
                                        '<option value="2">Директор</option>' +
                                      '</select>' +
                                    '</div>' +
                                  '</div>' +                                                                                                  
                                '</form>';
            
            $("#modalContent").html(html);
            return;
        case "editParameter":
            var html = '<form>' +
                                  '<div class="form-group row">' +
                                    '<label for="parValue" class="col-sm-2 col-form-label">Новое значение параметра</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="number" min="0" class="form-control-plaintext" id="parValue" style="min-width:100%" value="' + $(".selected").attr('curVal') + '">' +
                                    '</div>' +
                                  '</div>' +                                  
                                '</form>';

            $("#modalContent").html(html);
            return;
    }
}

function ViewReqs(type) {
    var data;

    switch (type) {
        case "0":
            data = null;
            break;
        case "1":
            data = { requestState: 2, withThisState: false };
            break;
        case "2":
            data = { requestState: 2, withThisState: true };
            break;
        case "3":
            data = { requestState: 3, withThisState: true };
            break;
        case "4":
            data = { requestState: 1, withThisState: true };
            break;
    }

    $.ajax(
            {
                url: "/api/ClientRequest",
                type: "GET",
                data: data,
                success: function (result) {
                    if (result != null)
                    {
                        var html = '';

                        //Кнопки
                        html += '<div class="col-md-12 col-lg-12 col-sm-12">' + '\n';
                        html += '<button type="button" class="btn btn-primary" id="createReq">Создать</button>' + '\n';
                        html += '<button type="button" class="btn btn-danger" id="cancelReq" disabled="true">Отменить</button>' + '\n';
                        html += '<button type="button" class="btn btn-primary" id="checkStatus" disabled="true">Проверить статус выбранной заявки</button>' + '\n';
                        html += '</div>' + '\n';

                        //Select
                        html += '<div class="col-md-6 col-lg-6 col-sm-6">' +
                                    '<label for="filterReq">Фильтр заявок</label>' +
                                    '<select class="form-control" id="filterReq">' +
                                        '<option value="0">Все</option>' +
                                        '<option value="1">Не выполненные</option>' +
                                        '<option value="2">Выполненные</option>' +
                                        '<option value="3">Отмененные</option>' +
                                        '<option value="4">Выполняются</option>' +
                                    '</select>' +
                                '</div>';

                        //Totals
                        html += '<div class="col-md-6 col-lg-6 col-sm-6">' +
                                    '<label for="totalCnt">Итоговое количество</label>' +
                                    '<input type="text" class="form-control" readonly id="totalCnt" value="' + result.length + '">' +
                                '</div>';
                        

                        //Таблица
                        html += '<table class="table">' + '<thead class="thead-dark">' + '<tr>';

                        //Заголовок
                        html += '<th>Id</th>' + '\n';
                        html += '<th>Текст заявки</th>' + '\n';
                        html += '<th>Создана</th>' + '\n';
                        html += '<th>Состояние</th>' + '\n';
                        html += '<th>Время выполнения</th>' + '\n';
                        html += '<th>Начало выполнения</th>' + '\n';
                        html += '<th>Исполнитель</th>' + '\n';
                        html += '<th>Должность</th>' + '\n';

                        html += '</tr>' + '</thead>' + '<tbody>' + '\n';

                        //Строки
                        $.each(result, function (index, value) {
                            html += '<tr id="req_' + result[index].ID + '" class="request">' + '\n';

                            html += '<th>' + result[index].ID + '</th>' + '\n';
                            html += '<td>' + result[index].Text + '</td>' + '\n';
                            html += '<td>' + result[index].CreatedOn.replace(/T/g," ") + '</td>' + '\n';
                            html += '<td>' + RequestStateString(result[index].State) + '</td>' + '\n';
                            html += '<td>' + result[index].ProcessTime + '</td>' + '\n';
                            html += '<td>' + DateTimeFormat(result[index].StartTime) + '</td>' + '\n';
                            html += '<td>' + ExecutorName(result[index].Executor) + '</td>' + '\n';
                            html += '<td>' + ExecutorTitle(result[index].Executor) + '</td>' + '\n';

                            html += '</tr>' + '\n';
                        });

                        html += '</tbody>' + '</table>';

                        $("#dataArea").html(html);
                        $("#filterReq").val(type);
                    }
                }
            });    
}

function ViewEmps(type) {
    var data;

    switch (type) {
        case "0":            
            data = null;
            break;
        case "1":
            data = { status: 0, withThisState: false };
            break;
        case "2":
            data = { status: 0, withThisState: true };
            break;
        case "3":
            data = { status: 1, withThisState: true };
            break;
    }

    $.ajax(
            {
                url: "/api/Employee",
                type: "GET",
                data: data,
                success: function (result) {
                    if (result != null) {
                        var html = '<table class="table">' + '\n' + '<thead class="thead-dark">' + '\n' + '<tr>';

                        //Кнопки
                        html += '<div class="col-md-12 col-lg-12 col-sm-12">' + '\n';
                        html += '<button type="button" class="btn btn-primary" id="createEmp">Нанять нового сотрудника</button>' + '\n';
                        html += '<button type="button" class="btn btn-danger" id="cancelEmp" disabled="true">Уволить</button>' + '\n';
                        html += '</div>' + '\n';
                        
                        //Select
                        html += '<div class="col-md-6 col-lg-6 col-sm-6">' +
                                    '<label for="filterEmp">Фильтр сотрудников</label>' +
                                    '<select class="form-control" id="filterEmp">' +
                                        '<option value="0">Все</option>' +
                                        '<option value="1">Свободные</option>' +
                                        '<option value="2">Занятые</option>' +
                                        '<option value="3">Уволенные</option>' +
                                    '</select>' +
                                '</div>';

                        //Totals
                        html += '<div class="col-md-6 col-lg-6 col-sm-6">' +
                                    '<label for="totalCnt">Итоговое количество</label>' +
                                    '<input type="text" class="form-control" readonly id="totalCnt" value="' + result.length + '">' +
                                '</div>';

                        //Заголовок
                        html += '<th>Id</th>' + '\n';
                        html += '<th>Имя</th>' + '\n';
                        html += '<th>Должность</th>' + '\n';
                        html += '<th>Занят/Свободен</th>' + '\n';
                        html += '<th>Уволен</th>' + '\n';
                        html += '<th>Число выполненных заявок</th>' + '\n';                        

                        html += '</tr>' + '\n' + '</thead>' + '\n' + '<tbody>' + '\n';

                        //Строки
                        $.each(result, function (index, value) {
                            html += '<tr id="emp_' + result[index].ID + '" class="employee">' + '\n';

                            html += '<th>' + result[index].ID + '</th>' + '\n';
                            html += '<td>' + result[index].Name + '</td>' + '\n';
                            html += '<td>' + ExecutorTitleString(result[index].Title) + '</td>' + '\n';                            
                            html += '<td>' + BusyString(result[index].IsBusy) + '</td>' + '\n';
                            html += '<td>' + FireString(result[index].IsFired) + '</td>' + '\n';
                            html += '<td>' + result[index].ReqCnt + '</td>' + '\n';

                            html += '</tr>' + '\n';
                        });

                        html += '</tbody>' + '\n' + '</table>';

                        $("#dataArea").html(html);
                        $("#filterEmp").val(type);
                    }
                }
            });
}

function ViewPars() {
    $.ajax(
            {
                url: "/api/SupportParameter",
                type: "GET",                
                success: function (result) {
                    if (result != null) {
                        var html = '<table class="table">' + '\n' + '<thead class="thead-dark">' + '\n' + '<tr>';

                        //Кнопки
                        html += '<div class="col-md-12 col-lg-12 col-sm-12">' + '\n';                        
                        html += '<button type="button" class="btn btn-primary" id="editPar" disabled="true">Изменить</button>' + '\n';
                        html += '</div>' + '\n';
                        
                        //Заголовок
                        html += '<th>Id</th>' + '\n';
                        html += '<th>Параметр</th>' + '\n';
                        html += '<th>Значение</th>' + '\n';                       

                        html += '</tr>' + '\n' + '</thead>' + '\n' + '<tbody>' + '\n';

                        //Строки
                        $.each(result, function (index, value) {
                            html += '<tr id="par_' + result[index].ID + '" curVal="' + result[index].ParameterValue + '" class="parameter">' + '\n';

                            html += '<th>' + result[index].ID + '</th>' + '\n';
                            html += '<td>' + result[index].ParameterName + '</td>' + '\n';
                            html += '<td>' + result[index].ParameterValue + '</td>' + '\n';

                            html += '</tr>' + '\n';
                        });

                        html += '</tbody>' + '\n' + '</table>';

                        $("#dataArea").html(html);                        
                    }
                }
            });
}

function Create(type) {
    switch (type) {
        case "createRequest":
            CreateRequest($("#requestText").val());
            return;
        case "createEmployee":
            CreateEmployee($("#empName").val(), $("#empTitle").val());
            return;
        case "editParameter":
            var id = $(".selected").attr('id');
            var val = parseInt($("#parValue").val());
            if (val < 0)
            {
                alert("Значение не поддерживается")
                throw "Значение не поддерживается";
            }

            EditParameter(id.substr(4), val);
            return;
    }
}

function CreateRequest(textValue) {
    var dataJSON = { text: textValue };

    $.ajax(
            {
                url: "/api/ClientRequest",
                data: JSON.stringify(dataJSON),
                dataType: 'json',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result != null && result.ID != null)
                    {
                        alert("Создана заявка с ID = " + result.ID);

                        ViewReqs($("#filterReq").val());
                    }
                }
            });    
}

function CreateEmployee(name, title) {
    var dataJSON = { Name: name, Title: title };

    $.ajax(
            {
                url: "/api/Employee",
                data: JSON.stringify(dataJSON),
                dataType: 'json',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result != null && result.ID != null) {
                        alert("Нанят сотрудник " + result.Name);

                        ViewEmps($("#filterEmp").val());
                    }
                }
            });    
}

function CancelRequest(id)
{
    $.ajax(
            {
                url: "/api/ClientRequest/" + id,
                type: "DELETE",
                success: function (result) {
                    if (result != null && result.ID != null) {
                        alert("Отменена заявка с ID = " + result.ID);

                        ViewReqs($("#filterReq").val());
                    }                    
                },
                error: function (jqXHR, exception) {
                    alert("Заявку уже невозможно отменить");
                }
            });    
}

function CancelEmployee(id) {
    $.ajax(
            {
                url: "/api/Employee/" + id,
                type: "DELETE",
                success: function (result) {
                    if (result != null && result.ID != null) {
                        alert("Уволен сотрудник " + result.Name);

                        ViewEmps($("#filterEmp").val());
                    }
                },
                error: function (jqXHR, exception) {
                    alert("Сотрудника не получилось уволить");
                }
            });    
}

function EditParameter(id, value) {
    var dataJSON = { ParameterValue: parseInt(value) };
    var data = { Id: id, supportParameter: JSON.stringify(dataJSON) };    

    $.ajax(
            {
                url: "/api/SupportParameter/" + id,
                type: "PUT",
                data: JSON.stringify(dataJSON),
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    alert("Параметр изменен на " + value);

                    ViewPars();
                },
                error: function (jqXHR, exception) {
                    alert("Не получилось");
                }
            });    
}

function CheckStatusRequest(id) {
    $.ajax(
            {
                url: "/api/ClientRequest/" + id,
                type: "GET",
                success: function (result) {
                    if (result != null && result.ID != null) {
                        var html = '<form>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqid" class="col-sm-2 col-form-label">ID</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqid" value="' + result.ID + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqState" class="col-sm-2 col-form-label">State</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqState" value="' + RequestStateString(result.State) + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +                                  
                                  '<div class="form-group row">' +
                                    '<label for="reqText" class="col-sm-2 col-form-label">Text</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqText" value="' + result.Text + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqCreatedOn" class="col-sm-2 col-form-label">CreatedOn</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqCreatedOn" value="' + DateTimeFormat(result.CreatedOn) + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqProcessTime" class="col-sm-2 col-form-label">ProcessTime</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqProcessTime" value="' + result.ProcessTime + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                  '<div class="form-group row">' +
                                    '<label for="reqStartTime" class="col-sm-2 col-form-label">StartTime</label>' +
                                    '<div class="col-sm-10">' +
                                      '<input type="text" readonly class="form-control-plaintext" id="reqStartTime" value="' + DateTimeFormat(result.StartTime) + '" style="min-width:100%">' +
                                    '</div>' +
                                  '</div>' +
                                '</form>';

                        $("#modalInfo").html(html);
                        $('#infoModal').modal("show");
                    }
                },
                error: function (jqXHR, exception) {
                    alert("Не смог :(");
                }
            });
}

function RequestStateString(state) {
    switch (state) {
        case 0: return 'Новая';
        case 1: return 'Выполняется';
        case 2: return 'Выполнена';
        case 3: return 'Отменена';        
    }
}

function DateTimeFormat(dateTime)
{
    return (dateTime != null ? dateTime.replace(/T/g, " ") : "")
}

function ExecutorName(executor)
{
    return (executor != null ? executor.Name : "");
}

function ExecutorTitle(executor) {
    return (executor != null ? ExecutorTitleString(executor.Title) : "");
}

function ExecutorTitleString(title) {
    switch (title) {
        case 0: return 'Оператор';
        case 1: return 'Менеджер';
        case 2: return 'Директор';        
    }
}

function BusyString(isBusy) {
    if (isBusy) {
        return "Занят";
    }
    else {
        return "Свободен";
    }
}

function FireString(isFired) {
    if (isFired) {
        return "Уволен";
    }

    return "";
}

var common = null;

$().ready(function () {
    common = new Common();
    common.init();
});