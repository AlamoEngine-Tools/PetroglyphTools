namespace PG.StarWarsGame.Files.ChunkFiles.Binary.Metadata;

public enum ChunkType
{
    Unknown,
    Skeleton = 0x200,
    BoneCount = 0x201,
    Bone = 0x202,
    BoneName = 0x203,
    Mesh = 0x400,
    MeshName = 0x401,
    MeshInformation = 0x402,
    Light = 0x1300,
    Connections = 0x600,
    ConnectionCounts = 0x601,
    ObjectConnection = 0x602,
    ProxyConnection = 0x603,
    Dazzle = 0x604,
    Particle = 0x900,
    Animation = 0x1000,
    ParticleUaW = 0x1500,
    SubMeshData = 0x00010000,
    SubMeshMaterialInformation = 0x00010100,
    ShaderFileName = 0x00010101,
    ShaderTexture = 0x00010105,
}