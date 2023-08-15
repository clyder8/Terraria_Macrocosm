﻿namespace Macrocosm.Content.Rockets.Customization
{
	public interface IUnlockable
	{
		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; }

		// public void OnUnlocked() { }
		public string GetKey();
	}
}
