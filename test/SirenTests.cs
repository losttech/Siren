namespace tensorflow.keras {
    using System;
    using System.Drawing;
    using System.Linq;
    using numpy;
    using tensorflow.keras.layers;
    using tensorflow.keras.optimizers;
    using Xunit;
    using static ImageTools;

    public partial class SirenTests {
        [Fact]
        public Model CanLearn() {
            tf.random.set_seed(11241);

            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string wikiLogoName = thisAssembly.GetManifestResourceNames()[0];

            using var wikiLogo = new Bitmap(thisAssembly.GetManifestResourceStream(wikiLogoName));
            byte[,,] bytesHWC = ToBytesHWC(wikiLogo);

            ndarray trainImage = NormalizeChannelValue(bytesHWC.ToNumPyArray())
                .reshape(new[] { wikiLogo.Width * wikiLogo.Height, 4 });
            var coords = Coord(wikiLogo.Height, wikiLogo.Width)
                .ToNumPyArray().reshape(new []{ wikiLogo.Height * wikiLogo.Width, 2});

            var model = new Sequential(new object[] {
                new Siren(2, Enumerable.Repeat(128, 3).ToArray()),
                new Dense(units: 4, activation: tf.keras.activations.relu_fn),
            });

            model.compile(
                optimizer: new Adam(),
                loss: "mse");

            model.fit(coords, targetValues: trainImage, epochs: 100, batchSize: wikiLogo.Height * wikiLogo.Width, stepsPerEpoch: 1024);

            double testLoss = model.evaluate(coords, trainImage);
            Assert.True(testLoss < 0.3, testLoss.ToString());
            return model;
        }
    }
}
