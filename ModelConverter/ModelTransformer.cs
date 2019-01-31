using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter
{
    public static class ModelTransformer
    {

        #region Public Methods

        public static IModel Transform(IModel model, Matrix transformation)
        {
            var result = new Model.Model();

            TransformVertices(model, transformation, result);
            TransformVertexNormals(model, transformation, result);
            TransformFaces(model, transformation, result);

            return result;
        }

        #endregion

        #region Private Methods

        private static void TransformFaces(IModel model, Matrix transformation, Model.Model result)
        {
            foreach (var f in model.Faces)
            {
                result.AddFace(new Face
                    {
                        Normal = transformation.Transform(f.Normal),
                        VertexIndices = f.VertexIndices,
                        NormalIndices = f.NormalIndices,
                        TextureCoordIndices = f.TextureCoordIndices
                    }
                );
            }
        }

        private static void TransformVertexNormals(IModel model, Matrix transformation, Model.Model result)
        {
            foreach (var vn in model.VertexNormals)
            {
                result.AddVertexNormal(transformation.Transform(vn));
            }
        }

        private static void TransformVertices(IModel model, Matrix transformation, Model.Model result)
        {
            foreach (var v in model.Vertices)
            {
                result.AddVertex(transformation.Transform(v));
            }
        }

        #endregion

    }
}