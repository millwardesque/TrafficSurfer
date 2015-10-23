
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_HighScore : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		HighScore data = (HighScore)obj;

		// Add your writer.Write calls here.
		writer.Write(data.name);
		writer.Write(data.score);

	}
	
	public override object Read(ES2Reader reader)
	{
		HighScore data = new HighScore();
		Read(reader, data);
		return data;
	}
	
	public override void Read(ES2Reader reader, object c)
	{
		HighScore data = (HighScore)c;
		// Add your reader.Read calls here to read the data into the object.
		data.name = reader.Read<System.String>();
		data.score = reader.Read<System.Int32>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_HighScore():base(typeof(HighScore)){}
}
