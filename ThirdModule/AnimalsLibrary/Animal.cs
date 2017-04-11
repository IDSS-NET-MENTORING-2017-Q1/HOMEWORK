using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnimalsLibrary
{
	[ComVisible(true)]
	[Guid("1C7D8685-64F1-4149-9004-A30D3C02065C")]
	[ClassInterface(ClassInterfaceType.None)]
	public class Animal : ICreature
	{
		protected const int SATIETY_REPLENISHMENT_VALUE = 15;
		protected const int HEALTH_REPLENISHMENT_VALUE = 15;

		protected const double SATIETY_PER_SECOND = 0.5;
		protected const double HEALTH_PER_SECOND = 0.1;

		protected int CurrentSatiety = 100;
		protected int CurrentHealth = 100;

		protected DateTime SatietyCheckDate;
		protected DateTime HealthCheckDate;

		public Animal()
		{
			SatietyCheckDate = DateTime.Now;
			HealthCheckDate = DateTime.Now;
		}

		public int GetSatiety()
		{
			var satietyExpired = (int) ((DateTime.Now - SatietyCheckDate).TotalSeconds * SATIETY_PER_SECOND);
			if (satietyExpired > CurrentSatiety)
				satietyExpired = CurrentSatiety;

			if (satietyExpired > 0)
				SatietyCheckDate = DateTime.Now;

			CurrentSatiety -= satietyExpired;
			return CurrentSatiety;
		}

		public void Feed()
		{
			var shortage = 100 - CurrentSatiety;
			var replenishment = (SATIETY_REPLENISHMENT_VALUE > shortage) ? shortage : SATIETY_REPLENISHMENT_VALUE;

			CurrentSatiety += replenishment;
			SatietyCheckDate = DateTime.Now;
		}

		public int GetHealth()
		{
			var healthExpired = (int)((DateTime.Now - HealthCheckDate).TotalSeconds * HEALTH_PER_SECOND);
			if (healthExpired > CurrentHealth)
				healthExpired = CurrentHealth;
			
			if (healthExpired > 0)
				HealthCheckDate = DateTime.Now;

			CurrentHealth -= healthExpired;

			return CurrentHealth;
		}

		public void Heal()
		{
			var shortage = 100 - CurrentHealth;
			var replenishment = (SATIETY_REPLENISHMENT_VALUE > shortage) ? shortage : HEALTH_REPLENISHMENT_VALUE;

			CurrentHealth += replenishment;
			HealthCheckDate = DateTime.Now;
		}


		public virtual string GetVoice()
		{
			throw new NotImplementedException();
		}
	}
}
