
(function () {
  $.fn.DMSFormValidator = function (FormNameID, ErrorContainerID) {
    var $form = $(FormNameID);

    $form.formValidation({
      framework: 'bootstrap',
      err: {
        container: (ErrorContainerID == null || ErrorContainerID.length == 0) ? null : ErrorContainerID
      },
      icon: {
        valid: 'glyphicon glyphicon-ok',
        invalid: 'glyphicon glyphicon-remove',
        validating: 'glyphicon glyphicon-refresh'
      },
      fields: {
        txtNetworkID: {
          message: 'The network ID is not valid',
          validators: {
            notEmpty: {
              message: 'The network ID is required'
            },
            stringLength: {
              min: 7,
              max: 30,
              message: 'The network ID must be more than 7 and less than 30 characters long'
            },
            regexp: {
              regexp: /^[a-zA-Z0-9_\.]+$/,
              message: 'The network ID can only consist of alphabetical, number, dot and underscore'
            }
          }
        },
        //txtEmail: {
        // validators: {
        //  notEmpty: {
        //   message: 'The email address is required'
        //  },
        //  emailAddress: {
        //   message: 'The input is not a valid email address'
        //  }
        // }
        //},
        txtPassword: {
          validators: {
            notEmpty: {
              message: 'The password is required'
            }
          }
        },
        TextRequiredFieldAcceptAll: {
          selector: '.TextRequiredFieldAcceptAll',
          validators: {
            notEmpty: {
              message: 'This field is required'
            }
          }
        },
        TextRequiredField: {
          selector: '.TextRequiredField',
          validators: {
            notEmpty: {
              message: 'This field is required'
            },
            regexp: {
              regexp: /^[a-zA-Z0-9_\. ]+$/,
              message: 'The accepts only consist of alphabetical, number, dot, underscore and space'
            }
          }
        },
        DateRequiredField: {
          selector: '.DateRequiredField',
          validators: {
            notEmpty: {
              message: 'Select a Date'
            }
          }
        },
        txtbranchCode: {
          validators: {
            notEmpty: {
              message: "Branch Code is Required"
            },
            integer: {
              message: "Branch Code can only contain numbers"
            }
          }
        },
        txtBranchCode: {
          validators: {
            notEmpty: {
              message: "Branch Code is Required"
            },
            integer: {
              message: "Branch Code can only contain numbers"
            }
          }
        },
        txtBranchName: {
          validators: {
            notEmpty: {
              message: "Branch Name is Required"
            }
          }
        },
        txtEmail: {
          validators: {
            notEmpty: {
              message: "Email is required"
            },
            //emailAddress: {
            //  message: 'The value is not a valid email address'
            //}
            regexp: {
              regexp: '^[^@\\s]+@([^@\\s]+\\.)+[^@\\s]+$',
              message: 'The value is not a valid email address'
            }
          }
        },
        txtBhossEmail: {
          validators: {
            notEmpty: {
              message: "BHOSS Email is required"
            },
            //emailAddress: {
            //  message: 'The value is not a valid email address'
            //}
            regexp: {
              regexp: '^[^@\\s]+@([^@\\s]+\\.)+[^@\\s]+$',
              message: 'The value is not a valid email address'
            }
          }
        },
        txtServiceName: {
          validators: {
            notEmpty: {
              message: "Service Type Name is Required"
            }
          }
        },
        txtInvestmentTypeCode: {
          validators: {
            notEmpty: {
              message: "Investment Type Code is Required"
            }
          }
        },
        txtInvestmentTypeDesc: {
          validators: {
            notEmpty: {
              message: "Investment Type Desc is Required"
            }
          }
        },
        txtInvestmentCode: {
          validators: {
            notEmpty: {
              message: "Investment Type Code is Required"
            }
          }
        },
        txtInvestmentDescription: {
          validators: {
            notEmpty: {
              message: "Investment Type Desc Required"
            }
          }
        },
        txtProductCode: {
          validators: {
            notEmpty: {
              message: "Product Code Required"
            },
            stringLength: {
              max: 4,
              message: "Cannot be more than 4 characters"
            },
            regexp: {
              regexp: /^\d+$/,
              message: "Cannot contain letters and special characters"
            }
          }
        },
        txtProductDesc: {
          validators: {
            notEmpty: {
              message: "Product Description Required"
            }
          }
        },
        txtCurrency: {
          validators: {
            notEmpty: {
              message: "Currency Required"
            },
            stringLength: {
              max: 43,
              message: "Cannot be more than 3 characters"
            }
          }
        },
        txtCost: {
          validators: {
            notEmpty: {
              message: "Cost Required"
            }
          }
        },
        txtType: {
          validators: {
            notEmpty: {
              message: "Type Required"
            }
          }
        },
        txtProductType: {
          validators: {
            notEmpty: {
              message: "Product Type Required"
            }
          }
        },
        txtProductGroup: {
          validators: {
            notEmpty: {
              message: "Product Group Required"
            }
          }
        }
      }
    });
    return $form;
  }
})(jQuery);