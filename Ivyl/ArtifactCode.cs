using RoR2;
using System.Security.Cryptography;

namespace IvyLibrary
{
    /// <summary>
    /// Represents a 3x3 artifact code as input for an artifact portal (e.g., on Sky Meadow).
    /// </summary>
    public struct ArtifactCode
    {
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) topRow;
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) middleRow;
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) bottomRow;

        public ArtifactCode(
            ArtifactCompound topLeft, ArtifactCompound topCenter, ArtifactCompound topRight, 
            ArtifactCompound middleLeft, ArtifactCompound middleCenter, ArtifactCompound middleRight, 
            ArtifactCompound bottomLeft, ArtifactCompound bottomCenter, ArtifactCompound bottomRight)
        {
            topRow = (topLeft, topCenter, topRight);
            middleRow = (middleLeft, middleCenter, middleRight);
            bottomRow = (bottomLeft, bottomCenter, bottomRight);
        }

        public ArtifactCode(
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) topRow, 
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) middleRow, 
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) bottomRow)
        {
            this.topRow = topRow;
            this.middleRow = middleRow;
            this.bottomRow = bottomRow;
        }

        public Sha256Hash CreateCodeHash()
        {
            using SHA256 hasher = SHA256.Create();
            byte[] buffer = new[]
            {
                (byte)topRow.Item3,
                (byte)middleRow.Item3,
                (byte)bottomRow.Item3,
                (byte)topRow.Item2,
                (byte)bottomRow.Item2,
                (byte)middleRow.Item2,
                (byte)topRow.Item1,
                (byte)middleRow.Item1,
                (byte)bottomRow.Item1,
            };
            return Sha256Hash.FromBytes(hasher.ComputeHash(buffer));
        }
    }
}