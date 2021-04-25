namespace TriggeredImagerNetFrwrkApp
{
    enum GEV_PIXEL_FORMAT
    {
        MONO8 = 0x01080001,
        MONO12 = 0x01100005,
        BAYER_GR_8 = 0x01080008,
        BAYER_RG_8 = 0x01080009,
        BAYER_GB_8 = 0x0108000A,
        BAYER_BG_8 = 0x0108000B,
        BAYER_BG_12 = 0x01100013,
        BAYER_BG_12_PACKED = 0x010C002D,
        YUV_422_8_UYVY = 0x0210001F,
        YUV_422_8 = 0x02100032,
        RGB8 = 0x02180014,
    };
}