﻿using HedgeLib.Bases;
using System.IO;
using System.Text;

namespace HedgeLib.Misc
{
	public class LWGism : FileBase
    {
		//Variables/Constants
		public LWFileBase LWFileData = new LWFileBase();
        public LWGismo[] Gismos;
        public uint UnknownBoolean1 = 1;

        public const string Signature = "GISM", Extension = ".gism";

        //Methods
        public override void Load(Stream fileStream)
        {
			//Header
			var reader = new ExtendedBinaryReader(fileStream, Encoding.ASCII, false);
			LWFileData.InitRead(reader);

			uint gismOffset = reader.ReadUInt32();
            UnknownBoolean1 = reader.ReadUInt32(); //TODO: Find out what this is.
			uint gismoCount = reader.ReadUInt32();
            Gismos = new LWGismo[gismoCount];

			//Containers
			uint containerOffset = reader.ReadUInt32();
            reader.JumpTo(containerOffset, false);

            for (uint i = 0; i < gismoCount; ++i)
            {
                var gismo = new LWGismo();

				//Container 1
				uint fileNameOffset = reader.ReadUInt32();
				uint fileNameOffset2 = reader.ReadUInt32(); //TODO: Find out what this is for.
				uint unknownNameOffset = reader.ReadUInt32();
                gismo.Unknown1 = reader.ReadUInt32();

                gismo.Unknown2 = reader.ReadSingle();
                gismo.Unknown3 = reader.ReadSingle();
                gismo.DoesAnimate = (reader.ReadUInt32() == 1);
				uint havokOffset = reader.ReadUInt32();

                gismo.UnknownBoolean1 = (reader.ReadUInt32() == 1);
				uint containerTwoOffset = reader.ReadUInt32();

                //TODO: Remove this debug code
                if (fileNameOffset != fileNameOffset2)
                    System.Console.WriteLine("WARNING: fileNameOffset != fileNameOffset2 (" +
                        fileNameOffset + " vs. " + fileNameOffset2 + ")");

				long curPos = reader.BaseStream.Position;
                gismo.FileName = reader.GetString(fileNameOffset, true);

                //Havok Array
                reader.JumpTo(havokOffset, false);
				uint unknown10 = reader.ReadUInt32();

                //TODO: Remove this debug code
                if (unknown10 != 0)
                    System.Console.WriteLine("WARNING: Unknown10 != 0 (" +
                        unknown10 + ".)");

                gismo.HavokName = reader.GetString();

                //Container 2
                reader.JumpTo(containerTwoOffset, false);

                gismo.UnknownBoolean2 = (reader.ReadUInt32() == 1);
                gismo.UnknownBoolean3 = (reader.ReadUInt32() == 1);

                gismo.Unknown5 = reader.ReadSingle();
                gismo.Unknown6 = reader.ReadSingle();
                gismo.Unknown7 = reader.ReadSingle();

                gismo.Unknown8 = reader.ReadSingle();
                gismo.RotationAmount = reader.ReadSingle();
                gismo.Unknown9 = reader.ReadSingle();

                reader.BaseStream.Position = curPos;
                Gismos[i] = gismo;
            }

			LWFileData.FinishRead(reader);
        }

        public override void Save(Stream fileStream)
        {
			//Header
			var writer = new ExtendedBinaryWriter(fileStream, Encoding.ASCII, false);
			LWFileData.InitWrite(writer);

			LWFileData.AddString(writer, "gismOffset", Signature);
            writer.Write(UnknownBoolean1);
            writer.Write((uint)Gismos.Length);

			//Containers
			LWFileData.AddOffset(writer, "containerOffset");
            writer.FillInOffset("containerOffset", false);

            //Container 1
            for (int i = 0; i < Gismos.Length; ++i)
            {
                var gismo = Gismos[i];

				LWFileData.AddString(writer, "fileNameOffset_" + i, gismo.FileName);
				LWFileData.AddString(writer, "fileNameOffset2_" + i, gismo.FileName);
				LWFileData.AddString(writer, "unknownOffset1_" + i, gismo.FileName); //TODO
                writer.Write(gismo.Unknown1);

                writer.Write(gismo.Unknown2);
                writer.Write(gismo.Unknown3);
                writer.Write((gismo.DoesAnimate) ? 1u : 0u);
				LWFileData.AddOffset(writer, "havokOffset_" + i);

                writer.Write((gismo.UnknownBoolean1) ? 1u : 0u);
				LWFileData.AddOffset(writer, "containerTwoOffset_" + i);
            }

            //Havok Array
            for (int i = 0; i < Gismos.Length; ++i)
            {
                writer.FillInOffset("havokOffset_" + i, false);
                writer.WriteNulls(4); //TODO: Figure out what this is
				LWFileData.AddString(writer, "havokNameOffset_" + i, Gismos[i].HavokName);
            }

            //Container 2
            for (int i = 0; i < Gismos.Length; ++i)
            {
                var gismo = Gismos[i];
                writer.FillInOffset("containerTwoOffset_" + i, false);

                writer.Write((gismo.UnknownBoolean2) ? 1u : 0u);
                writer.Write((gismo.UnknownBoolean3) ? 1u : 0u);

                writer.Write(gismo.Unknown5);
                writer.Write(gismo.Unknown6);
                writer.Write(gismo.Unknown7);

                writer.Write(gismo.Unknown8);
                writer.Write(gismo.RotationAmount);
                writer.Write(gismo.Unknown9);
            }

			LWFileData.FinishWrite(writer);
        }
    }

    public class LWGismo
    {
        //Variables/Constants
        public string FileName, HavokName;
        public float Unknown2, Unknown3, Unknown5,
            Unknown6, Unknown7, Unknown8,
            RotationAmount, Unknown9;

        public uint Unknown1 = 2;
        public bool DoesAnimate, UnknownBoolean1 = true,
            UnknownBoolean2 = true, UnknownBoolean3;
    }
}