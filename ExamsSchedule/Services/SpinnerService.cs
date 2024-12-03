using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class SpinnerService
    {
		public event Action OnShow;
		public event Action OnHide;

		public void Show()
		{
			OnShow?.Invoke();
		}

		public void Hide()
		{
			OnHide?.Invoke();
		}

	}
}
