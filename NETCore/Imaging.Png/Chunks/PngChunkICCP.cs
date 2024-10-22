namespace Hjg.Pngcs.Chunks;

/// <summary>
/// iCCP Chunk: see http://www.w3.org/TR/PNG/#11iCCP
/// </summary>
public class PngChunkICCP : PngChunkSingle
{
    public const string ID = ChunkHelper.iCCP;

    private string profileName;

    private byte[] compressedProfile;

    public PngChunkICCP(ImageInfo info)
        : base(ID, info)
    {
    }

    public override ChunkOrderingConstraint GetOrderingConstraint()
    {
        return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
    }

    public override ChunkRaw CreateRawChunk()
    {
        var c = CreateEmptyChunk(profileName.Length + compressedProfile.Length + 2, true);
        System.Array.Copy(Hjg.Pngcs.Chunks.ChunkHelper.ToBytes(profileName), 0, c.Data, 0, profileName.Length);
        c.Data[profileName.Length] = 0;
        c.Data[profileName.Length + 1] = 0;
        System.Array.Copy(compressedProfile, 0, c.Data, profileName.Length + 2, compressedProfile.Length);
        return c;
    }

    public override void ParseFromRaw(ChunkRaw c)
    {
        if (c is null)
        {
            throw new ArgumentNullException(nameof(c));
        }

        var pos0 = Hjg.Pngcs.Chunks.ChunkHelper.PosNullByte(c.Data);
        profileName = Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString(c.Data, 0, pos0);
        var comp = (c.Data[pos0 + 1] & 0xff);
        if (comp != 0)
        {
            throw new Exception("bad compression for ChunkTypeICCP");
        }

        var compdatasize = c.Data.Length - (pos0 + 2);
        compressedProfile = new byte[compdatasize];
        Array.Copy(c.Data, pos0 + 2, compressedProfile, 0, compdatasize);
    }

    public override void CloneDataFromRead(AbstractPngChunk other)
    {
        CloneData((PngChunkICCP)other);
    }

    private void CloneData(PngChunkICCP other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        profileName = other.profileName;
        compressedProfile = new byte[other.compressedProfile.Length];
        Array.Copy(other.compressedProfile, compressedProfile, compressedProfile.Length);
    }

    /// <summary>
    /// Sets profile name and profile
    /// </summary>
    /// <param name="name">profile name </param>
    /// <param name="profileName">profile (latin1 string)</param>
    public void SetProfileNameAndContent(string name, string profileName)
    {
        SetProfileNameAndContent(name, ChunkHelper.ToBytes(profileName));
    }

    /// <summary>
    /// Sets profile name and profile
    /// </summary>
    /// <param name="name">profile name </param>
    /// <param name="profile">profile (uncompressed)</param>
    public void SetProfileNameAndContent(string name, byte[] profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        profileName = name;
        compressedProfile = ChunkHelper.CompressBytes(profile, true);
    }

    public string GetProfileName()
    {
        return profileName;
    }

    /// <summary>
    /// This uncompresses the string!
    /// </summary>
    /// <returns></returns>
    public byte[] GetProfile()
    {
        return ChunkHelper.CompressBytes(compressedProfile, false);
    }

    public string GetProfileAsString()
    {
        return ChunkHelper.ToString(GetProfile());
    }
}