using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDM.BookVideo.Application.Interfaces.BusinessRules {
  public interface IBusinessRuleFactory {
    IBusinessRule GetBusinessRule();
  }
}
