public interface ICharge
{
	/// <summary>
	/// Gets or sets a value indicating whether this instance is attached to an inducer.
	/// When the instance is attached it will get a charge of the same sign as the inducer,
	/// otherwise it will get a charge of opposite sign.
	/// </summary>
	/// <value><c>true</c> if this instance is attached to an inducer; otherwise, <c>false</c>.</value>
	bool IsAttachedToInducer{ get; set; }
	float GetMaxCharge();
	float GetCharge();
	void SetCharge(float chargeStrength);
	void AddCharge(float chargeStrength);
	void Discharge();
}
