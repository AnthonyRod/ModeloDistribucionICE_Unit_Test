using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvcTemplate.Models.Indices;
using mvcTemplate.Models.Redistribucion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC_Core_SYbase.Models.Usuarios;
using MVC_Core_SYbase.Models.Pronosticos;
using ModeloDistribucionICE.Models.PuntoReorden;

namespace TestUnitarios
{
    [TestClass]
    public class TestIndices
    {
        private Indice Indices;

        public TestIndices()
        {
            Indices = new Indice();
        }
        [TestMethod]
        public void IndiceEfectividadVentas()
        {
            List<int> listaNumeros = new List<int>
            { 10, 1, 1, 99, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 9, 1, 88, 99, 2, 99, 99, 100 };
            Assert.AreEqual("3,29", Indices.eliminarDatosAtipicos(listaNumeros).ToString("0.00"));
            Assert.AreEqual(3, Indices.CalcularMediana(listaNumeros));
        }


        [TestMethod]
        public void IndiceIncidencias()
        {
            string skv = "HUAWEI MODELO U1000-5";
            string almacen = "AGENCIA BARRIO SAN JOSE";
            Indices.InicializarVariablesIndice(skv);
            Assert.AreEqual(100, Indices.Incidencia(almacen));

            skv = "DATACARD MODEM MARCA HUAWEI MODELO E1556";
            almacen = "AGENCIA SAN JOSE (AV. 5TA.)";
            Indices.InicializarVariablesIndice(skv);
            Assert.AreEqual("1,426", Indices.Incidencia(almacen).ToString("0.000"));
            //SELECT ALMACEN,COunt(SKV) FROM vista_historicos_ventas where MARCA= 'HUAWEI' and  GAMA = 'MEDIA ALTA' and ALMACEN = 'AGENCIA SAN JOSE (AV. 5TA.)' 
            //SELECT ALMACEN,COunt(SKV) FROM vista_incidencias where MARCA= 'HUAWEI' and  GAMA = 'MEDIA ALTA' and ALMACEN = 'AGENCIA SAN JOSE (AV. 5TA.)'


            //skv = "DATACARD MODEM MARCA HUAWEI MODELO E1556";
            //almacen = "AGENCIA SAN PEDRO - OUTLET MALL";
            //Indices.InicializarVariablesIndice(skv);
            //Assert.AreEqual("0,227", Indices.Incidencia(almacen).ToString("0.000"));
        }
        [TestMethod]
        public void IncideHistoricoVentas()
        {
            // parametros para realizar el reporte en la aplicacion : fecha actual = 1/3/2011 , fecha inicial = 31/12/2010 fecha final = 15/01/2011 ,  opotunidadventa dias = 15   GAMA = media alta 
            //en segunda patnatal seleccionar la primera opcion (372   de DATACARD MODEM MARCA HUAWEI MODELO E1556 )
            //luego para realizar redistribucion seleccionar la misma regio y 5 tiendas candidatas


            string skv = "DATACARD MODEM MARCA HUAWEI MODELO E1556";
            string almacen = "AGENCIA SAN ANTONIO DE CORONADO"; //  
            Indices.InicializarVariablesIndice(skv);
            //Indices.BasicCustomGet($"select from ")
            Assert.AreEqual("2,378", Indices.HistoricoDeVentas(almacen, Indices.DatosHistoricoVentas.Count()).ToString("0.000"));


            //skv = "DATACARD MODEM MARCA HUAWEI MODELO E1556";
            //almacen = "AGENCIA SAN PEDRO - OUTLET MALL"; //SELECT * FROM vista_historicos_ventas WHERE  FECHAREBAJA BETWEEN DATEADD(YY,-1,GETDATE()) and GETDATE() and MARCA = 'HUAWEI' and GAMA = 'MEDIA ALTA' and ALMACEN = 'AGENCIA SAN ANTONIO DE CORONADO' 
            //Indices.InicializarVariablesIndice(skv);
            //Assert.AreEqual("5,959", Indices.HistoricoDeVentas(almacen, Indices.DatosHistoricoVentas.Count()).ToString("0.000"));

        }
        //[TestMethod]
        //public void Login()
        //{
        //    MantenimientoUsuarios u = new MantenimientoUsuarios();
        //   // u.nombre = "ZAraneoS";
        //   // u.apellido = "roDRIGUE";
        //   // u.apellido2 = "cueta";
        //   // u.cedula = "11420909066";
        //   // u.email = "zaraneos@gmail.com";
        //   // u.id_rol = 3;
        //   // u.password = "lbareda";
        //   // u.nombreUsuario = "ELZARANEOS";
        //   //Assert.AreEqual(1, u.CrearUsuario());

        //    Assert.AreEqual(true, u.login("ELZARANEOS", "lbareda"));


        //}

        [TestMethod]
        public async Task TestMetodosPronosticos()
        {
            List<Periodos> lista = new List<Periodos>()
            {
                new Periodos { NumeroDePeriodo = 1, NumeroDeVentas = 15 } ,
                new Periodos { NumeroDePeriodo = 2, NumeroDeVentas = 21 },
                new Periodos { NumeroDePeriodo = 3, NumeroDeVentas = 21 },
                new Periodos { NumeroDePeriodo = 4, NumeroDeVentas = 20 },
                new Periodos { NumeroDePeriodo = 5, NumeroDeVentas = 22 },
                new Periodos { NumeroDePeriodo = 6, NumeroDeVentas = 16 },
                new Periodos { NumeroDePeriodo = 7, NumeroDeVentas = 20 },
                new Periodos { NumeroDePeriodo = 8, NumeroDeVentas = 15 },
                new Periodos { NumeroDePeriodo = 9, NumeroDeVentas = 17 },
        };

            DatosPorPeriodos datos = new DatosPorPeriodos
            {
                periocidad = "Mensual",
                listaPeriodos = lista
            };

            MetodoEstatico metodoEsta = new MetodoEstatico(datos);
            await metodoEsta.Calcular();

            SuavizacionExponencial suavizacion = new SuavizacionExponencial(datos);
            suavizacion.Calcular();

            RegresionLineal regresion = new RegresionLineal(datos);
            regresion.Calcular();

            SuavizacionExpDoble suaviDoble = new SuavizacionExpDoble(datos);
            suaviDoble.Calcular();

            MetodoHolt holt = new MetodoHolt(datos);
            holt.Calcular();

            PromedioMovil promedioMovil = new PromedioMovil(datos);
            promedioMovil.Calcular(7, 4);

            SuavizacionExpSimple suavisimple = new SuavizacionExpSimple(datos);
            suavisimple.Calcular();

            int desde = Convert.ToDateTime("1/2/2010").Year;
            int hasta = Convert.ToDateTime("3/3/2011").Year;

            ReordenInventarios r = new ReordenInventarios();
            r.InicializarVariables(2009, 2010, 9299, 15, 2, 1);
            var t = await r.CalcularPuntoReorden();
        }
    }
}