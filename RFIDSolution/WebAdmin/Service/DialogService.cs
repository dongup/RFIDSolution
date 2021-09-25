using Blazored.Modal;
using Blazored.Modal.Services;
using RFIDSolution.Shared.Models;
using RFIDSolution.WebAdmin.Modals;
using RFIDSolution.WebAdmin.Pages;
using RFIDSolution.WebAdmin.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Service
{
    public class DialogService
    {
        private IModalService _modal;
        public DialogService(IModalService modal)
        {
            _modal = modal;
        }

        public async Task<bool> ShowEditProductModal(ProductModel model)
        {
            var parameters = new ModalParameters();
            parameters.Add("item", model);

            var modal = _modal.Show<ProductEditModal>("Update shoe information", parameters);
            var res = await modal.Result;
            return res.Cancelled ? false : true;
        }

        public async Task<bool> Confirm(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);

            var modal = _modal.Show<ConfirmModal>("Confirm", parameters);
            var res = await modal.Result;
            return res.Cancelled ? false : true;
        }


        public async Task<bool> AlertModal(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);

            var modal = _modal.Show<AlertModal>("Alert", parameters);
            var res = await modal.Result;
            return res.Cancelled ? false : true;
        }

        public async Task SuccessAlert(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var dialog = MainLayout.Instance.AlertDialog;
            dialog.Success(message);
        }

        public async Task ErrorAlert(string message)
        {
            var dialog = MainLayout.Instance.AlertDialog;
            if (string.IsNullOrEmpty(message)) return;
            dialog.Error(message);
        }

        public async Task InfoAlert(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var dialog = MainLayout.Instance.AlertDialog;
            dialog.Info(message);
        }
    }
}
